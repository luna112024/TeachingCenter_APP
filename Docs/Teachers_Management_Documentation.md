# Teachers Management System Documentation

## Overview
The Teachers Management System handles teacher resources within the Hong Wen education platform. It provides comprehensive functionality for managing teacher profiles, qualifications, schedules, workload, and employment information.

## Business Workflow

### 1. Teacher Lifecycle
```
User Registration → Teacher Profile Creation → Qualification Setup → Schedule Assignment → Active Teaching → Performance Tracking
```

### 2. Key Business Rules
- **Teacher Codes**: Must be unique across the system (e.g., "T001", "TCH001")
- **User Integration**: Each teacher must be linked to a system user account
- **Chinese Proficiency**: Required field for language teaching capability
- **Experience Tracking**: Years of teaching experience for qualification assessment
- **Workload Management**: Maximum hours per week to prevent overloading
- **Employment Status**: Active, Inactive, On Leave, Terminated tracking
- **Contract Management**: Full-time, Part-time, Contract, Freelance options

### 3. User Roles and Permissions
- **ViewTeacher**: Can view teacher information and schedules
- **ManageTeacher**: Can create and edit teacher profiles
- **DeleteTeacher**: Can delete teachers (separate permission for data protection)

## API Endpoints

### Base URL: `/sms/Teacher`

| Method | Endpoint | Description | Parameters | Permission |
|--------|----------|-------------|------------|------------|
| GET | `/` | Get all teachers | `name` | ViewTeacher |
| GET | `/{teacherId}` | Get specific teacher | `teacherId` (GUID) | ViewTeacher |
| GET | `/by-user/{userId}` | Get teacher by user ID | `userId` (GUID) | ViewTeacher |
| GET | `/available` | Get available teachers | `from`, `to` (dates) | ViewTeacher |
| GET | `/{teacherId}/workload` | Get teacher workload | `teacherId` (GUID) | ViewTeacher |
| GET | `/{teacherId}/schedule` | Get teacher schedule | `teacherId` (GUID) | ViewTeacher |
| POST | `/` | Create new teacher | CreateTeacherDTO in body | ManageTeacher |
| PUT | `/` | Update existing teacher | UpdateTeacherDTO in body | ManageTeacher |
| DELETE | `/{teacherId}` | Delete teacher | `teacherId` (GUID) | DeleteTeacher |

### Query Parameters
- **name**: Filter by teacher name (partial match supported)
- **from**: Start date for availability check (YYYY-MM-DD format)
- **to**: End date for availability check (YYYY-MM-DD format)

## Data Transfer Objects (DTOs)

### TeacherBaseDTO (Abstract)
```csharp
public abstract record class TeacherBaseDTO
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required, StringLength(20)]
    public string TeacherCode { get; set; } = string.Empty;
    
    public string? Specializations { get; set; }
    public string? Qualifications { get; set; }
    
    [Range(0, int.MaxValue)]
    public int? ExperienceYears { get; set; }
    
    [Required, StringLength(20)]
    public string ChineseProficiency { get; set; } = string.Empty;
    
    public string? TeachingLanguages { get; set; }
    
    [Range(1, int.MaxValue)]
    public int? MaxHoursPerWeek { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? HourlyRate { get; set; }
    
    [Required]
    public DateTime HireDate { get; set; }
    
    [StringLength(20)]
    public string? ContractType { get; set; }
    
    [StringLength(20)]
    public string? EmploymentStatus { get; set; }
    
    public string? Notes { get; set; }
}
```

### CreateTeacherDTO
- Inherits from TeacherBaseDTO
- Additional: `CreatedBy` (auto-populated from JWT claims)

### UpdateTeacherDTO
- Inherits from TeacherBaseDTO
- Additional: `TeacherId`, `ModifiedBy` (auto-populated from JWT claims)

### GetTeacherDTO
- Inherits from TeacherBaseDTO
- Additional: `TeacherId`, `CreatedBy`, `ModifiedBy`, `CreateDate`, `UpdateDate`
- Display fields: `UserName`, `UserEmail`, `UserPhone` (from linked user account)

## Application Layer Usage

### Controller Actions

#### 1. Index (Main View)
- **Route**: `GET /Teacher`
- **Purpose**: Display paginated list of all teachers
- **Features**: Search functionality, pagination, teacher creation
- **Permissions**: ViewTeacher

#### 2. ListTeacher (AJAX Partial View)
- **Route**: `GET /Teacher/ListTeacher`
- **Purpose**: Refresh teacher list with search filters
- **Parameters**: `ListTeacherDTOs` (includes SearchText, Page, PageSize)
- **Returns**: Partial view with filtered results

#### 3. FetchTeacher (API for JavaScript)
- **Route**: `GET /Teacher/FetchTeacher`
- **Purpose**: Get teacher data for JavaScript/AJAX calls
- **Parameters**: `i` (teacher name filter)
- **Returns**: JSON array of teacher objects

#### 4. AddTeacher
- **GET Route**: `GET /Teacher/AddTeacher` - Show create form
- **POST Route**: `POST /Teacher/AddTeacher` - Process creation
- **Model**: `CreateTeacherDTO`
- **Features**: User integration, qualification tracking
- **Permissions**: ManageTeacher

#### 5. EditTeacher
- **GET Route**: `GET /Teacher/EditTeacher/{id}` - Show edit form
- **POST Route**: `POST /Teacher/EditTeacher` - Process update
- **Model**: `UpdateTeacherDTO`
- **Features**: Pre-populated form, validation
- **Permissions**: ManageTeacher

#### 6. DeleteTeacher
- **GET Route**: `GET /Teacher/DeleteTeacher/{id}` - Show confirmation
- **POST Route**: `POST /Teacher/DeleteTeacherConfirmed` - Process deletion
- **Permissions**: DeleteTeacher

#### 7. GetAvailableTeachers
- **Route**: `GET /Teacher/GetAvailableTeachers`
- **Purpose**: Get available teachers for scheduling
- **Returns**: JSON array for AJAX consumption
- **Used by**: Class scheduling, assignment

#### 8. GetTeacherWorkload
- **Route**: `GET /Teacher/GetTeacherWorkload/{id}`
- **Purpose**: Get teacher workload information
- **Returns**: JSON object with workload data

#### 9. GetTeacherSchedule
- **Route**: `GET /Teacher/GetTeacherSchedule/{id}`
- **Purpose**: Get teacher schedule and classes
- **Returns**: JSON object with schedule data

## Frontend Implementation

### Views Structure
```
Views/Teacher/
├── Index.cshtml                 # Main teachers listing page
├── _ListTeachers.cshtml        # Partial view for teachers table
├── _addTeacher.cshtml          # Modal form for creating teachers
├── _editTeacher.cshtml         # Modal form for editing teachers
└── _deleteTeacher.cshtml       # Modal confirmation for deletion
```

### Key Features

#### 1. Search and Filtering
- **Current Implementation**: Search by teacher name via SearchText
- **API Capability**: Supports name filtering
- **Enhancement Opportunity**: Could implement advanced filtering by proficiency, experience, status

#### 2. Pagination
- Uses `PageList<GetTeacherDTO>` for pagination
- Configurable page size (default: 10, options: 20, 50, 100)
- Maintains search filters across pages

#### 3. Status Display
- **Active**: Green badge for active teachers
- **Inactive**: Gray badge for inactive teachers
- **On Leave**: Yellow badge for teachers on leave
- **Terminated**: Red badge for terminated teachers

#### 4. Actions Menu
- **Edit**: Available for all teachers
- **Schedule**: View teacher schedule and classes
- **Workload**: View teacher workload information
- **Delete**: Available with proper permissions

#### 5. Form Features
- **User Integration**: Link to existing user accounts
- **Teacher Code**: Unique identifier for each teacher
- **Chinese Proficiency**: Required proficiency level selection
- **Experience Tracking**: Years of teaching experience
- **Workload Management**: Maximum hours per week
- **Rate Management**: Hourly rate for payment calculation
- **Employment Details**: Contract type and employment status
- **Qualification Tracking**: Specializations, qualifications, teaching languages
- **Notes**: Additional information about the teacher

## Service Layer

### TeacherService Implementation
```csharp
public class TeacherService : BaseApiService, ITeacherService
{
    // Implements all CRUD operations
    // Handles HTTP communication with API
    // Manages query parameter construction
    // Provides typed responses
}
```

### Key Methods
- `GetAllTeachers()`: Retrieve teachers with optional name filter
- `GetTeacher()`: Get specific teacher by ID
- `GetTeacherByUserId()`: Get teacher by linked user ID
- `GetAvailableTeachers()`: Get teachers available for scheduling
- `GetTeacherWorkload()`: Get teacher workload information
- `GetTeacherSchedule()`: Get teacher schedule and classes
- `CreateTeacher()`: Create new teacher
- `UpdateTeacher()`: Update existing teacher
- `DeleteTeacher()`: Delete teacher by ID

## Business Logic

### 1. Teacher Creation Workflow
1. User clicks "Add Teacher" button
2. Modal opens with teacher creation form
3. User fills required information:
   - User ID (link to existing user account)
   - Teacher Code (unique identifier)
   - Chinese Proficiency (required for language teaching)
   - Hire Date (employment start date)
   - Experience, qualifications, specializations
   - Workload and rate information
4. System validates business rules
5. Teacher profile created with specified properties
6. Available for class assignment and scheduling

### 2. User Integration
- Each teacher must be linked to a system user account
- User account provides authentication and basic profile information
- Teacher profile extends user account with teaching-specific data
- Enables unified login and profile management

### 3. Chinese Proficiency Levels
- **Native**: Native Chinese speaker
- **Advanced**: High-level Chinese proficiency
- **Intermediate**: Moderate Chinese proficiency
- **Beginner**: Basic Chinese proficiency

### 4. Employment Management
- **Contract Types**: Full-time, Part-time, Contract, Freelance
- **Employment Status**: Active, Inactive, On Leave, Terminated
- **Workload Tracking**: Maximum hours per week to prevent overloading
- **Rate Management**: Hourly rate for payment calculation

### 5. Qualification Tracking
- **Specializations**: Areas of expertise (e.g., HSK Preparation, Business Chinese)
- **Qualifications**: Educational background and certifications
- **Teaching Languages**: Languages the teacher can instruct in
- **Experience**: Years of teaching experience

## Validation Rules

### Business Validation
- Teacher codes must be unique
- User ID must reference existing user account
- Chinese proficiency is required
- Hire date must be valid
- Experience years must be non-negative
- Max hours per week must be positive
- Hourly rate must be non-negative

### Form Validation
- Required fields enforced
- String length limits
- Numeric range validations
- Date validations
- Client-side real-time validation
- Server-side validation as backup

## Integration Points

- **User Management**: Integration with user account system
- **Class Scheduling**: Teachers assigned to scheduled classes
- **Student Management**: Teacher-student relationships
- **Payment System**: Hourly rate integration for payroll
- **Performance Tracking**: Teacher evaluation and feedback
- **Reporting**: Teacher utilization and performance analytics

## Performance Considerations

- **Pagination**: Large teacher catalogs handled efficiently
- **Caching**: Consider caching available teachers for frequent access
- **Indexing**: Database indexes on frequently queried fields
- **Lazy Loading**: Partial views for better page load times
- **Search Optimization**: Efficient filtering and search

## Security Considerations

- **Authentication**: All endpoints require valid JWT token
- **Authorization**: Role-based permissions enforced
- **Input Validation**: Server-side validation on all DTOs
- **CSRF Protection**: Anti-forgery tokens on all forms
- **Audit Trail**: CreatedBy/ModifiedBy tracking on all changes
- **Data Protection**: Soft delete for teacher history preservation
- **User Privacy**: Sensitive information properly protected

## Reporting and Analytics

- Teacher utilization statistics
- Workload distribution analysis
- Performance metrics and evaluations
- Qualification and experience reports
- Employment status tracking
- Payment and rate analysis

## Future Enhancements

1. **Advanced Search**: Multi-criteria search with all filter options
2. **Teacher Templates**: Predefined teacher profiles for quick creation
3. **Bulk Operations**: Mass update teacher information
4. **Performance Tracking**: Integrated teacher evaluation system
5. **Integration APIs**: External system integration capabilities
6. **Mobile Optimization**: Responsive design improvements
7. **Real-time Status**: Live availability updates
8. **Workload Analytics**: Advanced workload distribution reporting
9. **Qualification Management**: Detailed qualification tracking
10. **Payment Integration**: Direct payroll system integration

## Usage Examples

### Creating a New Teacher
1. Navigate to Teachers management
2. Click "Add Teacher" button
3. Fill in teacher details:
   - User ID: Select from existing users
   - Code: "T001"
   - Chinese Proficiency: "Native"
   - Experience: 5 years
   - Specializations: "HSK Preparation, Business Chinese"
4. Set employment details and rates
5. Save teacher profile

### Using Teachers in Class Scheduling
1. Open Class scheduling form
2. Available teachers load via AJAX
3. Select appropriate teacher for the class
4. System checks availability and workload
5. Teacher is assigned to the class

### Checking Teacher Availability
1. Admin checks teacher availability
2. System filters by date range and status
3. Available teachers displayed
4. Workload and qualifications considered
5. Scheduling conflicts prevented

## API Response Examples

### GetAvailableTeachers Response
```json
[
  {
    "id": "guid-here",
    "text": "T001 - John Doe",
    "code": "T001",
    "name": "John Doe",
    "email": "john.doe@example.com",
    "chineseProficiency": "Native",
    "experienceYears": 5,
    "employmentStatus": "Active"
  }
]
```

### GetTeacher Response
```json
{
  "teacherId": "guid-here",
  "userId": "user-guid-here",
  "teacherCode": "T001",
  "userName": "John Doe",
  "userEmail": "john.doe@example.com",
  "chineseProficiency": "Native",
  "experienceYears": 5,
  "specializations": "HSK Preparation, Business Chinese",
  "qualifications": "Master's in Chinese Language, HSK Level 6",
  "maxHoursPerWeek": 40,
  "hourlyRate": 25.00,
  "hireDate": "2023-01-15",
  "contractType": "Full-time",
  "employmentStatus": "Active"
}
```

### GetTeacherSchedule Response
```json
[
  {
    "classId": "guid-here",
    "className": "Chinese Level 1",
    "startTime": "09:00",
    "endTime": "10:30",
    "date": "2024-01-15",
    "classroom": "A101",
    "studentCount": 25
  }
]
```

### GetTeacherWorkload Response
```json
{
  "currentWeekHours": 35,
  "maxHoursPerWeek": 40,
  "utilizationPercentage": 87.5,
  "classesThisWeek": 14,
  "averageClassSize": 22
}
```

This comprehensive Teacher management system provides a robust foundation for teacher resource administration within the Hong Wen education platform.
