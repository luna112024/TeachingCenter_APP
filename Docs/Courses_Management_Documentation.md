# Courses Management System Documentation

## Overview
The Courses Management System handles course catalog management within the Hong Wen education platform. It provides comprehensive functionality for creating, managing, and tracking courses with their levels, pricing, duration, and capacity settings.

## Business Workflow

### 1. Course Lifecycle
```
Draft → Active → Enrollment → Running → Completed → Archive
```

### 2. Key Business Rules
- **Course Codes**: Must be unique across the system
- **Level Integration**: Courses are linked to predefined levels (Beginner, Elementary, Intermediate, Advanced, HSK1-HSK6)
- **Capacity Management**: Each course has minimum and maximum student limits
- **Pricing Structure**: Base fee + optional materials fee
- **Duration Calculation**: Total hours = Duration weeks × Hours per week
- **Age Group Targeting**: Courses can target specific age groups (Kids, Teens, Adults, All Ages)
- **Status Management**: Courses can be Active, Inactive, or Draft

### 3. User Roles and Permissions
- **ViewCourse**: Can view course information and listings
- **ManageCourse**: Can create, edit, and duplicate courses
- **DeleteCourse**: Can delete courses (separate permission for data protection)

## API Endpoints

### Base URL: `/sms/Course`

| Method | Endpoint | Description | Parameters | Permission |
|--------|----------|-------------|------------|------------|
| GET | `/` | Get all courses | `level`, `ageGroup`, `status`, `name`, `code` | ViewCourse |
| GET | `/{courseId}` | Get specific course | `courseId` (GUID) | ViewCourse |
| GET | `/by-level/{level}` | Get courses by level name | `level` (string) | ViewCourse |
| GET | `/by-level-id/{levelId}` | Get courses by level ID | `levelId` (GUID) | ViewCourse |
| POST | `/` | Create new course | CreateCourseDTO in body | ManageCourse |
| PUT | `/` | Update existing course | UpdateCourseDTO in body | ManageCourse |
| DELETE | `/{courseId}` | Delete course | `courseId` (GUID) | DeleteCourse |
| POST | `/{courseId}/duplicate` | Duplicate course | `courseId` (GUID), DuplicateCourseDTO in body | ManageCourse |

### Query Parameters
- **level**: Filter by level name (e.g., "Beginner", "HSK1")
- **ageGroup**: Filter by age group ("Kids", "Teens", "Adults", "All Ages")
- **status**: Filter by status ("Active", "Inactive", "Draft")
- **name**: Filter by course name (partial match supported)
- **code**: Filter by course code (exact match)

## Data Transfer Objects (DTOs)

### CourseBaseDTO (Abstract)
```csharp
public abstract record class CourseBaseDTO
{
    [Required, StringLength(20)]
    public string CourseCode { get; set; }
    
    [Required, StringLength(200)]
    public string CourseName { get; set; }
    
    [StringLength(200)]
    public string? CourseNameChinese { get; set; }
    
    public string? Description { get; set; }
    public string? DescriptionChinese { get; set; }
    
    [Required]
    public Guid LevelId { get; set; }
    
    [Required, Range(1, int.MaxValue)]
    public int DurationWeeks { get; set; }
    
    [Required, Range(1, int.MaxValue)]
    public int HoursPerWeek { get; set; }
    
    [Required, Range(1, int.MaxValue)]
    public int TotalHours { get; set; }
    
    [Range(1, int.MaxValue)]
    public int MaxStudents { get; set; } = 20;
    
    [Range(1, int.MaxValue)]
    public int MinStudents { get; set; } = 5;
    
    [StringLength(20)]
    public string AgeGroup { get; set; } = "All Ages";
    
    public string? Prerequisites { get; set; }
    public string? LearningOutcomes { get; set; }
    public string? MaterialsIncluded { get; set; }
    
    [Required, Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal BaseFee { get; set; }
    
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal MaterialsFee { get; set; } = 0.00m;
    
    [StringLength(20)]
    public string Status { get; set; } = "Active";
}
```

### CreateCourseDTO
- Inherits from CourseBaseDTO
- Additional: `CreatedBy` (auto-populated from JWT claims)

### UpdateCourseDTO
- Inherits from CourseBaseDTO
- Additional: `CourseId`, `ModifiedBy` (auto-populated from JWT claims)

### GetCourseDTO
- Inherits from CourseBaseDTO
- Additional: `CourseId`, `CreatedBy`, `ModifiedBy`, `CreateDate`, `UpdateDate`
- Display fields: `LevelName`, `LevelCode`, `LevelNameChinese`

### DuplicateCourseDTO
```csharp
public record class DuplicateCourseDTO
{
    [Required, StringLength(20)]
    public string NewCourseCode { get; set; }
}
```

## Application Layer Usage

### Controller Actions

#### 1. Index (Main View)
- **Route**: `GET /Course`
- **Purpose**: Display paginated list of all courses
- **Features**: Search functionality, pagination, course creation
- **Permissions**: ViewCourse

#### 2. ListCourse (AJAX Partial View)
- **Route**: `GET /Course/ListCourse`
- **Purpose**: Refresh course list with search filters
- **Parameters**: `ListCourseDTOs` (includes SearchText, Page, PageSize)
- **Returns**: Partial view with filtered results

#### 3. FetchCourse (API for JavaScript)
- **Route**: `GET /Course/FetchCourse`
- **Purpose**: Get course data for JavaScript/AJAX calls
- **Parameters**: `i` (course name filter)
- **Returns**: JSON array of course objects

#### 4. AddCourse
- **GET Route**: `GET /Course/AddCourse` - Show create form
- **POST Route**: `POST /Course/AddCourse` - Process creation
- **Model**: `CreateCourseDTO`
- **Features**: Auto-calculate total hours, validation
- **Permissions**: ManageCourse

#### 5. EditCourse
- **GET Route**: `GET /Course/EditCourse/{id}` - Show edit form
- **POST Route**: `POST /Course/EditCourse` - Process update
- **Model**: `UpdateCourseDTO`
- **Features**: Pre-populated form, validation
- **Permissions**: ManageCourse

#### 6. DeleteCourse
- **GET Route**: `GET /Course/DeleteCourse/{id}` - Show confirmation
- **POST Route**: `POST /Course/DeleteCourseConfirmed` - Process deletion
- **Permissions**: DeleteCourse

#### 7. DuplicateCourse
- **GET Route**: `GET /Course/DuplicateCourse/{id}` - Show duplication form
- **POST Route**: `POST /Course/DuplicateCourseConfirmed` - Process duplication
- **Model**: `DuplicateCourseDTO`
- **Features**: Copy all course details with new code
- **Permissions**: ManageCourse

#### 8. GetCoursesByLevel
- **Route**: `GET /Course/GetCoursesByLevel`
- **Purpose**: Get courses filtered by level name
- **Returns**: JSON array of courses

#### 9. GetCoursesByLevelId
- **Route**: `GET /Course/GetCoursesByLevelId`
- **Purpose**: Get courses filtered by level ID
- **Returns**: JSON array of courses

## Frontend Implementation

### Views Structure
```
Views/Course/
├── Index.cshtml                 # Main courses listing page
├── _ListCourses.cshtml         # Partial view for courses table
├── _addCourse.cshtml           # Modal form for creating courses
├── _editCourse.cshtml          # Modal form for editing courses
├── _deleteCourse.cshtml        # Modal confirmation for deletion
└── _duplicateCourse.cshtml     # Modal form for duplication
```

### Key Features

#### 1. Search and Filtering
- **Current Implementation**: Search by course name via SearchText
- **API Capability**: Supports level, ageGroup, status, name, and code filters
- **Enhancement Opportunity**: Could implement advanced filtering UI

#### 2. Pagination
- Uses `PageList<GetCourseDTO>` for pagination
- Configurable page size (default: 10, options: 20, 50, 100)
- Maintains search filters across pages

#### 3. Status Display
- **Active**: Green badge for active courses
- **Draft**: Yellow badge for draft courses
- **Inactive**: Gray badge for inactive courses

#### 4. Actions Menu
- **Edit**: Available for all courses
- **Duplicate**: Create copy with new course code
- **Delete**: Available with proper permissions

#### 5. Form Features
- **Auto-calculation**: Total hours = Duration weeks × Hours per week
- **Validation**: Client and server-side validation
- **Dropdowns**: Predefined options for Level, Age Group, Status
- **Bilingual Support**: Chinese name and description fields

## Service Layer

### CourseService Implementation
```csharp
public class CourseService : BaseApiService, ICourseService
{
    // Implements all CRUD operations
    // Handles HTTP communication with API
    // Manages query parameter construction
    // Provides typed responses
}
```

### Key Methods
- `GetAllCourses()`: Retrieve courses with optional filters
- `GetCourse()`: Get specific course by ID
- `GetCoursesByLevel()`: Get courses by level name
- `GetCoursesByLevelId()`: Get courses by level ID
- `CreateCourse()`: Create new course
- `UpdateCourse()`: Update existing course
- `DeleteCourse()`: Delete course by ID
- `DuplicateCourse()`: Create course copy with new code

## Business Logic

### 1. Course Creation Workflow
1. User clicks "Add Course" button
2. Modal opens with course creation form
3. User fills required information:
   - Course Code (unique)
   - Course Name (English/Chinese)
   - Level selection
   - Duration and hours (auto-calculates total)
   - Capacity limits
   - Pricing information
4. System validates business rules
5. Course created with "Draft" status by default
6. Can be activated when ready for enrollment

### 2. Course Duplication
1. Select existing course from list
2. Click "Duplicate" action
3. System copies all course details
4. User provides new unique course code
5. New course created as copy with "Draft" status
6. Useful for creating similar courses or new terms

### 3. Level Integration
- Courses are linked to predefined levels
- Level determines course difficulty and prerequisites
- Used for student progression tracking
- Enables level-based filtering and reporting

### 4. Pricing Structure
- **Base Fee**: Core course price
- **Materials Fee**: Optional additional fee for materials
- **Total Cost**: Base + Materials fees
- Supports decimal precision for accurate pricing

## Validation Rules

### Business Validation
- Course codes must be unique
- Total hours must equal Duration × Hours per week
- Maximum students must be greater than minimum students
- Base fee must be greater than 0
- Materials fee must be 0 or greater

### Form Validation
- Required fields enforced
- String length limits
- Numeric range validations
- Client-side real-time validation
- Server-side validation as backup

## Integration Points

- **Level Management**: Courses linked to level system
- **Student Enrollment**: Course capacity affects enrollment
- **Class Scheduling**: Course duration used for scheduling
- **Fee Management**: Course pricing integrated with billing
- **Reporting**: Course metrics and analytics
- **Academic Planning**: Course progression paths

## Performance Considerations

- **Pagination**: Large course catalogs handled efficiently
- **Caching**: Consider caching course lists for frequent access
- **Indexing**: Database indexes on frequently queried fields
- **Lazy Loading**: Partial views for better page load times
- **Search Optimization**: Efficient filtering and search

## Security Considerations

- **Authentication**: All endpoints require valid JWT token
- **Authorization**: Role-based permissions enforced
- **Input Validation**: Server-side validation on all DTOs
- **CSRF Protection**: Anti-forgery tokens on all forms
- **Audit Trail**: CreatedBy/ModifiedBy tracking on all changes
- **Data Protection**: Soft delete for course history preservation

## Reporting and Analytics

- Course popularity metrics
- Enrollment vs capacity analysis
- Revenue per course tracking
- Level distribution analysis
- Age group preferences
- Course completion rates
- Fee structure effectiveness

## Future Enhancements

1. **Advanced Search**: Multi-criteria search with level, age group, status filters
2. **Course Templates**: Predefined course templates for quick creation
3. **Bulk Operations**: Mass update course information
4. **Course Versioning**: Track course changes over time
5. **Integration APIs**: External system integration capabilities
6. **Mobile Optimization**: Responsive design improvements
7. **Internationalization**: Full multi-language support
8. **Course Prerequisites**: Automatic prerequisite checking
9. **Capacity Alerts**: Notifications when courses near capacity
10. **Dynamic Pricing**: Time-based or demand-based pricing models
```

The documentation reveals that the main issues were missing endpoints (`GetCoursesByLevelId`), incorrect endpoint URLs, and DTO mismatches between API and APP. The Course management system is otherwise well-structured and follows the established patterns.
