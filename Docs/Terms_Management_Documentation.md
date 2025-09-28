# Terms Management System Documentation

## Overview
The Terms Management System handles academic terms/semesters within the Hong Wen education platform. It provides comprehensive functionality for creating, managing, and tracking academic terms with their associated dates, registration periods, and current term designation.

## Business Workflow

### 1. Term Lifecycle
```
Creation → Active → Registration Period → Term Period → Completion → Archive
```

### 2. Key Business Rules
- **Only one current term**: Only one term can be marked as "current" at any time
- **Registration periods**: Each term has specific registration start/end dates
- **Academic year grouping**: Terms are organized by academic year (e.g., "2024-2025")
- **Status management**: Terms can be Active, Inactive, or Completed
- **Date validation**: End dates must be after start dates, registration periods must be logical

### 3. User Roles and Permissions
- **ViewTerm**: Can view term information and listings
- **ManageTerm**: Can create, edit terms, and set current term
- **DeleteTerm**: Can delete terms (separate permission for data protection)

## API Endpoints

### Base URL: `/sms/Term`

| Method | Endpoint | Description | Parameters | Permission |
|--------|----------|-------------|------------|------------|
| GET | `/` | Get all terms | `academicYear`, `termName`, `termCode` | ViewTerm |
| GET | `/active` | Get active terms | `academicYear`, `termName`, `termCode` | ViewTerm |
| GET | `/current` | Get current term | None | ViewTerm |
| GET | `/{termId}` | Get specific term | `termId` (GUID) | ViewTerm |
| POST | `/` | Create new term | CreateTermDTO in body | ManageTerm |
| PUT | `/` | Update existing term | UpdateTermDTO in body | ManageTerm |
| DELETE | `/{termId}` | Delete term | `termId` (GUID) | DeleteTerm |
| POST | `/{termId}/set-current` | Set term as current | `termId` (GUID) | ManageTerm |

### Query Parameters
- **academicYear**: Filter by academic year (e.g., "2024-2025")
- **termName**: Filter by term name (partial match supported)
- **termCode**: Filter by term code (exact match)

## Data Transfer Objects (DTOs)

### TermBaseDTO (Abstract)
```csharp
public abstract record class TermBaseDTO
{
    [Required, StringLength(100)]
    public string TermName { get; set; }
    
    [Required, StringLength(20)]
    public string TermCode { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    public DateTime RegistrationStart { get; set; }
    
    [Required]
    public DateTime RegistrationEnd { get; set; }
    
    public string Status { get; set; } = "Active";
    public bool Iscurrent { get; set; } = false;
    
    [Required, StringLength(20)]
    public string AcademicYear { get; set; }
}
```

### CreateTermDTO
- Inherits from TermBaseDTO
- Additional: `CreatedBy` (auto-populated from JWT claims)

### UpdateTermDTO
- Inherits from TermBaseDTO
- Additional: `TermId`, `ModifiedBy` (auto-populated from JWT claims)

### GetTermDTO
- Inherits from TermBaseDTO
- Additional: `TermId`, `CreatedBy`, `ModifiedBy`, `CreateDate`, `UpdateDate`

## Application Layer Usage

### Controller Actions

#### 1. Index (Main View)
- **Route**: `GET /Term`
- **Purpose**: Display paginated list of all terms
- **Features**: Search functionality, pagination
- **Permissions**: ViewTerm

#### 2. ListTerm (AJAX Partial View)
- **Route**: `GET /Term/ListTerm`
- **Purpose**: Refresh term list with search filters
- **Parameters**: `ListTermDTOs` (includes SearchText, Page, PageSize)
- **Returns**: Partial view with filtered results

#### 3. FetchTerm (API for JavaScript)
- **Route**: `GET /Term/FetchTerm`
- **Purpose**: Get term data for JavaScript/AJAX calls
- **Parameters**: `i` (term name filter)
- **Returns**: JSON array of term objects

#### 4. AddTerm
- **GET Route**: `GET /Term/AddTerm` - Show create form
- **POST Route**: `POST /Term/AddTerm` - Process creation
- **Model**: `CreateTermDTO`
- **Permissions**: ManageTerm

#### 5. EditTerm
- **GET Route**: `GET /Term/EditTerm/{id}` - Show edit form
- **POST Route**: `POST /Term/EditTerm` - Process update
- **Model**: `UpdateTermDTO`
- **Permissions**: ManageTerm

#### 6. DeleteTerm
- **GET Route**: `GET /Term/DeleteTerm/{id}` - Show confirmation
- **POST Route**: `POST /Term/DeleteTermConfirmed` - Process deletion
- **Permissions**: DeleteTerm

#### 7. SetCurrentTerm
- **GET Route**: `GET /Term/SetCurrentTerm/{id}` - Show confirmation
- **POST Route**: `POST /Term/SetCurrentTermConfirmed` - Process setting
- **Permissions**: ManageTerm

## Frontend Implementation

### Views Structure
```
Views/Term/
├── Index.cshtml                 # Main terms listing page
├── _ListTerms.cshtml           # Partial view for terms table
├── _addTerm.cshtml             # Modal form for creating terms
├── _editTerm.cshtml            # Modal form for editing terms
├── _deleteTerm.cshtml          # Modal confirmation for deletion
└── _setCurrentTerm.cshtml      # Modal confirmation for setting current
```

### Key Features

#### 1. Search and Filtering
- **Current Implementation**: Search by term name via SearchText
- **API Capability**: Supports academicYear, termName, and termCode filters
- **Issue**: APP currently maps SearchText incorrectly to academicYear parameter

#### 2. Pagination
- Uses `PageList<GetTermDTO>` for pagination
- Configurable page size (default: 10)
- Maintains search filters across pages

#### 3. Status Display
- **Current**: Blue badge for current term
- **Active**: Green badge for active terms
- **Others**: Gray badge for inactive/completed terms

#### 4. Actions Menu
- **Edit**: Available for all terms
- **Set Current**: Only available for non-current terms
- **Delete**: Available for all terms (with permission check)

## Service Layer

### TermService Implementation
```csharp
public class TermService : BaseApiService, ITermService
{
    // Implements all CRUD operations
    // Handles HTTP communication with API
    // Manages query parameter construction
    // Provides typed responses
}
```

### Key Methods
- `GetAllTerms()`: Retrieve terms with optional filters
- `GetActiveTerm()`: Get only active terms
- `GetCurrentTerm()`: Get the currently designated term
- `CreateTerm()`: Create new term
- `UpdateTerm()`: Update existing term
- `DeleteTerm()`: Delete term by ID
- `SetCurrentTerm()`: Designate term as current

## Issues and Recommendations

### Current Issues

1. **Query Parameter Mapping**
   ```csharp
   // Current (INCORRECT)
   var terms = await _termService.GetAllTerms(model.SearchText);
   
   // Should be (CORRECT)
   var terms = await _termService.GetAllTerms(termName: model.SearchText);
   ```

2. **Missing Filter Usage**
   - Index method doesn't utilize search filters from model
   - Could implement advanced filtering for academicYear and termCode

### Recommendations

1. **Fix Parameter Mapping**
   - Update ListTerm method to properly map SearchText to termName parameter
   - Update Index method to use filters when provided

2. **Enhanced Search Functionality**
   - Add separate fields for academicYear, termName, and termCode filters
   - Implement advanced search modal with all filter options

3. **Validation Improvements**
   - Add client-side date validation (end > start, registration periods logical)
   - Implement business rule validation (only one current term)

4. **User Experience Enhancements**
   - Add confirmation when setting current term (already implemented)
   - Show term duration and registration period in listings
   - Add term status transitions (Active → Completed)

## Usage Examples

### Creating a New Term
1. User clicks "Add Term" button
2. Modal opens with `_addTerm.cshtml` form
3. User fills required fields:
   - Term Name: "Fall Semester 2024"
   - Term Code: "FALL2024"
   - Academic Year: "2024-2025"
   - Start/End dates and registration periods
4. Form submits to `POST /Term/AddTerm`
5. Controller validates and calls API
6. Success/error response handled by ReturnHelper

### Setting Current Term
1. User clicks "Set Current" from actions menu
2. Confirmation modal shows term details
3. User confirms action
4. API call automatically unsets previous current term
5. New term becomes current, affecting system-wide operations

### Search and Filter
1. User enters search text in main search box
2. Form submits to `List
