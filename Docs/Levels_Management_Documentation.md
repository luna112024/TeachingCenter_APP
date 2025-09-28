# Levels Management System Documentation

## Overview
The Levels Management System handles academic levels within the Hong Wen education platform. It provides comprehensive functionality for creating, managing, and tracking academic levels with their associated HSK levels, CEFR equivalents, and hierarchical structure.

## Business Workflow

### 1. Level Lifecycle
```
Creation → Active → Course Assignment → Student Placement → Archive
```

### 2. Key Business Rules
- **Level Codes**: Must be unique across the system (e.g., "HSK1", "BEG", "ELEM")
- **Level Order**: Sequential ordering for progression (1, 2, 3, 4...)
- **HSK Integration**: Levels can be mapped to HSK levels (HSK1-HSK6)
- **CEFR Mapping**: Levels can be mapped to CEFR equivalents (A1-C2)
- **Age Group Targeting**: Levels can target specific age groups (Kids, Teens, Adults)
- **Hierarchical Structure**: Support for parent-child level relationships
- **Placement Testing**: Levels can be used in student placement tests

### 3. User Roles and Permissions
- **ViewLevel**: Can view level information and listings
- **ManageLevel**: Can create and edit levels
- **DeleteLevel**: Can delete levels (separate permission for data protection)

## API Endpoints

### Base URL: `/sms/Level`

| Method | Endpoint | Description | Parameters | Permission |
|--------|----------|-------------|------------|------------|
| GET | `/` | Get all levels | `levelCode`, `levelName`, `isActive`, `isForBeginner`, `isForPlacement`, `hskLevel`, `cefrEquivalent` | ViewLevel |
| GET | `/list` | Get levels list | `levelCode`, `levelName`, `isActive`, `hskLevel` | ViewLevel |
| GET | `/hierarchy` | Get levels hierarchy | None | ViewLevel |
| GET | `/{levelId}` | Get specific level | `levelId` (GUID) | ViewLevel |
| GET | `/by-parent/{parentLevelId?}` | Get levels by parent | `parentLevelId` (GUID, optional) | ViewLevel |
| GET | `/active` | Get active levels | None | ViewLevel |
| GET | `/placement` | Get levels for placement | None | ViewLevel |
| GET | `/beginner` | Get beginner levels | None | ViewLevel |
| POST | `/` | Create new level | CreateLevelDTO in body | ManageLevel |
| PUT | `/` | Update existing level | UpdateLevelDTO in body | ManageLevel |
| DELETE | `/{levelId}` | Delete level | `levelId` (GUID) | DeleteLevel |
| PUT | `/reorder` | Reorder levels | List<Guid> in body | ManageLevel |
| GET | `/validate/code/{levelCode}` | Validate level code | `levelCode`, `excludeLevelId` (optional) | ViewLevel |
| GET | `/validate/name/{levelName}` | Validate level name | `levelName`, `excludeLevelId` (optional) | ViewLevel |

### Query Parameters
- **levelCode**: Filter by level code (e.g., "HSK1", "BEG")
- **levelName**: Filter by level name (partial match supported)
- **isActive**: Filter by active status (true/false)
- **isForBeginner**: Filter by beginner flag (true/false)
- **isForPlacement**: Filter by placement test eligibility (true/false)
- **hskLevel**: Filter by HSK level ("HSK1"-"HSK6")
- **cefrEquivalent**: Filter by CEFR equivalent ("A1"-"C2")

## Data Transfer Objects (DTOs)

### LevelBaseDTO (Abstract)
```csharp
public abstract record class LevelBaseDTO
{
    [Required, StringLength(10)]
    public string LevelCode { get; set; } = string.Empty;
    
    [Required, StringLength(50)]
    public string LevelName { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? LevelNameChinese { get; set; }
    
    [StringLength(50)]
    public string? LevelNameKhmer { get; set; }
    
    [Required, Range(1, int.MaxValue)]
    public int LevelOrder { get; set; }
    
    public Guid? ParentLevelId { get; set; }
    
    public string? Description { get; set; }
    public string? DescriptionChinese { get; set; }
    public string? Prerequisites { get; set; }
    public string? LearningObjectives { get; set; }
    
    [Range(1, int.MaxValue)]
    public int? ExpectedHours { get; set; }
    
    [StringLength(10)]
    public string? HskLevel { get; set; }
    
    [StringLength(5)]
    public string? CefrEquivalent { get; set; }
    
    [StringLength(20)]
    public string? MinAgeGroup { get; set; }
    
    [StringLength(20)]
    public string? MaxAgeGroup { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool IsForBeginner { get; set; } = false;
    public bool IsForPlacement { get; set; } = true;
}
```

### CreateLevelDTO
- Inherits from LevelBaseDTO
- Additional: `CreatedBy` (auto-populated from JWT claims)

### UpdateLevelDTO
- Inherits from LevelBaseDTO
- Additional: `LevelId`, `ModifiedBy` (auto-populated from JWT claims)

### GetLevelDTO
- Inherits from LevelBaseDTO
- Additional: `LevelId`, `CreatedBy`, `ModifiedBy`, `CreateDate`, `UpdateDate`
- Display fields: `ParentLevelName`, `SubLevelsCount`, `CoursesCount`, `StudentsCount`

### LevelListDTO
```csharp
public record class LevelListDTO
{
    public Guid LevelId { get; set; }
    public string LevelCode { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public string? LevelNameChinese { get; set; }
    public string? LevelNameKhmer { get; set; }
    public int LevelOrder { get; set; }
    public string? ParentLevelName { get; set; }
    public string? HskLevel { get; set; }
    public string? CefrEquivalent { get; set; }
    public bool IsActive { get; set; }
    public bool IsForBeginner { get; set; }
    public bool IsForPlacement { get; set; }
}
```

### LevelHierarchyDTO
```csharp
public record class LevelHierarchyDTO
{
    public Guid LevelId { get; set; }
    public string LevelCode { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public string? LevelNameChinese { get; set; }
    public int LevelOrder { get; set; }
    public Guid? ParentLevelId { get; set; }
    public bool IsActive { get; set; }
    public List<LevelHierarchyDTO> SubLevels { get; set; } = new List<LevelHierarchyDTO>();
}
```

## Application Layer Usage

### Controller Actions

#### 1. Index (Main View)
- **Route**: `GET /Level`
- **Purpose**: Display paginated list of all levels
- **Features**: Search functionality, pagination, level creation
- **Permissions**: ViewLevel

#### 2. ListLevel (AJAX Partial View)
- **Route**: `GET /Level/ListLevel`
- **Purpose**: Refresh level list with search filters
- **Parameters**: `ListLevelDTOs` (includes SearchText, Page, PageSize)
- **Returns**: Partial view with filtered results

#### 3. FetchLevel (API for JavaScript)
- **Route**: `GET /Level/FetchLevel`
- **Purpose**: Get level data for JavaScript/AJAX calls
- **Parameters**: `i` (level name filter)
- **Returns**: JSON array of level objects

#### 4. AddLevel
- **GET Route**: `GET /Level/AddLevel` - Show create form
- **POST Route**: `POST /Level/AddLevel` - Process creation
- **Model**: `CreateLevelDTO`
- **Features**: Multi-language support, HSK/CEFR mapping
- **Permissions**: ManageLevel

#### 5. EditLevel
- **GET Route**: `GET /Level/EditLevel/{id}` - Show edit form
- **POST Route**: `POST /Level/EditLevel` - Process update
- **Model**: `UpdateLevelDTO`
- **Features**: Pre-populated form, validation
- **Permissions**: ManageLevel

#### 6. DeleteLevel
- **GET Route**: `GET /Level/DeleteLevel/{id}` - Show confirmation
- **POST Route**: `POST /Level/DeleteLevelConfirmed` - Process deletion
- **Permissions**: DeleteLevel

#### 7. GetActiveLevels
- **Route**: `GET /Level/GetActiveLevels`
- **Purpose**: Get active levels for dropdowns
- **Returns**: JSON array for AJAX consumption
- **Used by**: Course forms, Student placement

#### 8. GetLevelsList
- **Route**: `GET /Level/GetLevelsList`
- **Purpose**: Get levels list for general use
- **Returns**: JSON array for AJAX consumption

#### 9. GetLevel
- **Route**: `GET /Level/GetLevel/{id}`
- **Purpose**: Get specific level details
- **Returns**: JSON object

## Frontend Implementation

### Views Structure
```
Views/Level/
├── Index.cshtml                 # Main levels listing page
├── _ListLevels.cshtml          # Partial view for levels table
├── _addLevel.cshtml            # Modal form for creating levels
├── _editLevel.cshtml           # Modal form for editing levels
└── _deleteLevel.cshtml         # Modal confirmation for deletion
```

### Key Features

#### 1. Search and Filtering
- **Current Implementation**: Search by level name via SearchText
- **API Capability**: Supports levelCode, levelName, isActive, hskLevel, cefrEquivalent filters
- **Enhancement Opportunity**: Could implement advanced filtering UI

#### 2. Pagination
- Uses `PageList<GetLevelDTO>` for pagination
- Configurable page size (default: 10, options: 20, 50, 100)
- Maintains search filters across pages

#### 3. Status Display
- **Active**: Green badge for active levels
- **Inactive**: Gray badge for inactive levels

#### 4. Actions Menu
- **Edit**: Available for all levels
- **Delete**: Available with proper permissions

#### 5. Form Features
- **Multi-language Support**: Chinese and Khmer name fields
- **HSK/CEFR Mapping**: Dropdown selections for standardized levels
- **Age Group Targeting**: Min/Max age group selection
- **Hierarchical Support**: Parent level selection
- **Validation**: Client and server-side validation
- **Checkboxes**: IsActive, IsForBeginner, IsForPlacement flags

## Service Layer

### LevelService Implementation
```csharp
public class LevelService : BaseApiService, ILevelService
{
    // Implements all CRUD operations
    // Handles HTTP communication with API
    // Manages query parameter construction
    // Provides typed responses
}
```

### Key Methods
- `GetAllLevels()`: Retrieve levels with optional filters
- `GetLevelsList()`: Get simplified level list
- `GetLevelsHierarchy()`: Get hierarchical level structure
- `GetLevel()`: Get specific level by ID
- `GetActiveLevels()`: Get only active levels
- `GetLevelsForPlacement()`: Get levels eligible for placement tests
- `GetBeginnerLevels()`: Get levels for absolute beginners
- `CreateLevel()`: Create new level
- `UpdateLevel()`: Update existing level
- `DeleteLevel()`: Delete level by ID
- `ReorderLevels()`: Reorder level sequence
- `IsLevelCodeUnique()`: Validate level code uniqueness
- `IsLevelNameUnique()`: Validate level name uniqueness

## Business Logic

### 1. Level Creation Workflow
1. User clicks "Add Level" button
2. Modal opens with level creation form
3. User fills required information:
   - Level Code (unique identifier)
   - Level Name (English/Chinese/Khmer)
   - Level Order (sequence number)
   - HSK Level and CEFR equivalent (optional)
   - Age group targeting
   - Description and learning objectives
4. System validates business rules
5. Level created with specified properties
6. Can be activated for course assignment

### 2. Level Hierarchy
- Levels can have parent-child relationships
- Useful for sub-levels (e.g., HSK1A, HSK1B)
- Supports complex academic progression paths
- Enables hierarchical display and filtering

### 3. HSK/CEFR Integration
- **HSK Levels**: HSK1-HSK6 for Chinese proficiency
- **CEFR Equivalents**: A1-C2 for European language framework
- Enables cross-referencing with international standards
- Supports placement test alignment

### 4. Age Group Targeting
- **Kids**: Ages 3-12
- **Teens**: Ages 13-17
- **Adults**: Ages 18+
- Helps match levels to appropriate age groups
- Supports age-appropriate content and teaching methods

## Validation Rules

### Business Validation
- Level codes must be unique
- Level names must be unique
- Level order must be positive integer
- HSK levels must be valid (HSK1-HSK6)
- CEFR equivalents must be valid (A1-C2)
- Age groups must be logical (MinAgeGroup ≤ MaxAgeGroup)

### Form Validation
- Required fields enforced
- String length limits
- Numeric range validations
- Client-side real-time validation
- Server-side validation as backup

## Integration Points

- **Course Management**: Levels linked to courses
- **Student Placement**: Levels used in placement tests
- **Academic Progression**: Levels define learning paths
- **Reporting**: Level-based analytics and reporting
- **Curriculum Planning**: Levels structure curriculum
- **Assessment**: Levels used for assessment frameworks

## Performance Considerations

- **Pagination**: Large level catalogs handled efficiently
- **Caching**: Consider caching active levels for frequent access
- **Indexing**: Database indexes on frequently queried fields
- **Lazy Loading**: Partial views for better page load times
- **Search Optimization**: Efficient filtering and search

## Security Considerations

- **Authentication**: All endpoints require valid JWT token
- **Authorization**: Role-based permissions enforced
- **Input Validation**: Server-side validation on all DTOs
- **CSRF Protection**: Anti-forgery tokens on all forms
- **Audit Trail**: CreatedBy/ModifiedBy tracking on all changes
- **Data Protection**: Soft delete for level history preservation

## Reporting and Analytics

- Level usage statistics
- Student progression through levels
- Course-level distribution analysis
- Placement test effectiveness
- Age group preferences
- HSK/CEFR alignment metrics

## Future Enhancements

1. **Advanced Search**: Multi-criteria search with all filter options
2. **Level Templates**: Predefined level templates for quick creation
3. **Bulk Operations**: Mass update level information
4. **Level Versioning**: Track level changes over time
5. **Integration APIs**: External system integration capabilities
6. **Mobile Optimization**: Responsive design improvements
7. **Internationalization**: Full multi-language support
8. **Level Prerequisites**: Automatic prerequisite checking
9. **Dynamic Ordering**: Drag-and-drop level reordering
10. **Level Analytics**: Advanced reporting and insights

## Usage Examples

### Creating a New Level
1. Navigate to Levels management
2. Click "Add Level" button
3. Fill in level details:
   - Code: "HSK1"
   - Name: "HSK Level 1"
   - Order: 1
   - HSK Level: "HSK1"
   - CEFR: "A1"
4. Set age group and description
5. Save level

### Using Levels in Course Creation
1. Open Course creation form
2. Level dropdown loads active levels via AJAX
3. Select appropriate level for the course
4. Course is linked to the selected level

### Student Placement
1. Admin runs placement test
2. System uses levels marked "IsForPlacement"
3. Student is assigned to appropriate level
4. Course recommendations based on level

## API Response Examples

### GetActiveLevels Response
```json
[
  {
    "id": "guid-here",
    "text": "HSK Level 1",
    "code": "HSK1",
    "hskLevel": "HSK1",
    "cefrEquivalent": "A1"
  }
]
```

### GetLevel Response
```json
{
  "levelId": "guid-here",
  "levelCode": "HSK1",
  "levelName": "HSK Level 1",
  "levelNameChinese": "汉语水平考试一级",
  "levelOrder": 1,
  "hskLevel": "HSK1",
  "cefrEquivalent": "A1",
  "isActive": true,
  "isForBeginner": true,
  "isForPlacement": true,
  "description": "Beginner level for Chinese language learning",
  "expectedHours": 40
}
```

This comprehensive Level management system provides a robust foundation for academic level administration within the Hong Wen education platform.
