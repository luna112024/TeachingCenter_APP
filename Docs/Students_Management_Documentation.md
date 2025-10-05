# Student Management System Documentation

## Overview
The Student Management System handles student profiles and information within the Hong Wen education platform. It provides comprehensive functionality for managing student data, academic records, contact information, and enrollment history.

## Business Workflow

### 1. Student Lifecycle
```
Student Registration → Profile Creation → Academic Information Setup → Enrollment Processing → Active Student → Graduation/Completion
```

### 2. Key Business Rules
- **Student Codes**: Must be unique across the system (e.g., "S001", "STU001")
- **User Integration**: Each student must be linked to a system user account
- **Academic Levels**: Students are assigned to appropriate academic levels
- **Contact Information**: Multiple contact methods for communication
- **Emergency Contacts**: Required for student safety and communication
- **Academic Status**: Active, Inactive, Graduated, Suspended, Withdrawn tracking
- **Enrollment History**: Complete record of all enrollments and academic progress

### 3. User Roles and Permissions
- **ViewStudent**: Can view student information and academic records
- **ManageStudent**: Can create and edit student profiles
- **DeleteStudent**: Can delete students (separate permission for data protection)

## API Endpoints

### Base URL: `/sms/Student`

| Method | Endpoint | Description | Parameters | Permission |
|--------|----------|-------------|------------|------------|
| GET | `/` | Get all students | `name`, `levelId`, `status` | ViewStudent |
| GET | `/{studentId}` | Get specific student | `studentId` (GUID) | ViewStudent |
| GET | `/by-user/{userId}` | Get student by user ID | `userId` (GUID) | ViewStudent |
| GET | `/by-level/{levelId}` | Get students by level | `levelId` (GUID) | ViewStudent |
| GET | `/by-status/{status}` | Get students by status | `status` (string) | ViewStudent |
| GET | `/{studentId}/enrollments` | Get student enrollments | `studentId` (GUID) | ViewStudent |
| GET | `/{studentId}/academic-record` | Get academic record | `studentId` (GUID) | ViewStudent |
| POST | `/` | Create new student | CreateStudentDTO in body | ManageStudent |
| PUT | `/` | Update existing student | UpdateStudentDTO in body | ManageStudent |
| DELETE | `/{studentId}` | Delete student | `studentId` (GUID) | DeleteStudent |

### Query Parameters
- **name**: Filter by student name (partial match supported)
- **levelId**: Filter by academic level (GUID)
- **status**: Filter by student status (Active, Inactive, Graduated, etc.)

## Data Transfer Objects (DTOs)

### StudentBaseDTO (Abstract)
```csharp
public abstract record class StudentBaseDTO
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required, StringLength(20)]
    public string StudentCode { get; set; } = string.Empty;
    
    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    public DateTime? DateOfBirth { get; set; }
    
    [StringLength(10)]
    public string? Gender { get; set; }
    
    [StringLength(20)]
    public string? Nationality { get; set; }
    
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    
    [StringLength(100)]
    public string? Email { get; set; }
    
    [StringLength(200)]
    public string? Address { get; set; }
    
    [StringLength(20)]
    public string? EmergencyContactName { get; set; }
    
    [StringLength(20)]
    public string? EmergencyContactPhone { get; set; }
    
    [StringLength(50)]
    public string? EmergencyContactRelationship { get; set; }
    
    [StringLength(20)]
    public string? AcademicStatus { get; set; }
    
    public Guid? CurrentLevelId { get; set; }
    
    public DateTime? EnrollmentDate { get; set; }
    
    public string? Notes { get; set; }
}
```

### CreateStudentDTO
- Inherits from StudentBaseDTO
- Additional: `CreatedBy` (auto-populated from JWT claims)

### UpdateStudentDTO
- Inherits from StudentBaseDTO
- Additional: `StudentId`, `ModifiedBy` (auto-populated from JWT claims)

### GetStudentDTO
- Inherits from StudentBaseDTO
- Additional: `StudentId`, `CreatedBy`, `ModifiedBy`, `CreateDate`, `UpdateDate`
- Display fields: `UserName`, `UserEmail`, `LevelName`, `EnrollmentCount`

## Application Layer Usage

### Controller Actions

#### 1. Index (Main View)
- **Route**: `GET /Student`
- **Purpose**: Display paginated list of all students
- **Features**: Search functionality, pagination, student creation
- **Permissions**: ViewStudent

#### 2. ListStudent (AJAX Partial View)
- **Route**: `GET /Student/ListStudent`
- **Purpose**: Refresh student list with search filters
- **Parameters**: `ListStudentDTOs` (includes SearchText, Page, PageSize)
- **Returns**: Partial view with filtered results

#### 3. FetchStudent (API for JavaScript)
- **Route**: `GET /Student/FetchStudent`
- **Purpose**: Get student data for JavaScript/AJAX calls
- **Parameters**: `i` (student name filter)
- **Returns**: JSON array of student objects

#### 4. AddStudent
- **GET Route**: `GET /Student/AddStudent` - Show create form
- **POST Route**: `POST /Student/AddStudent` - Process creation
- **Model**: `CreateStudentDTO`
- **Features**: User integration, academic level assignment
- **Permissions**: ManageStudent

#### 5. EditStudent
- **GET Route**: `GET /Student/EditStudent/{id}` - Show edit form
- **POST Route**: `POST /Student/EditStudent` - Process update
- **Model**: `UpdateStudentDTO`
- **Features**: Pre-populated form, validation
- **Permissions**: ManageStudent

#### 6. DeleteStudent
- **GET Route**: `GET /Student/DeleteStudent/{id}` - Show confirmation
- **POST Route**: `POST /Student/DeleteStudentConfirmed` - Process deletion
- **Permissions**: DeleteStudent

#### 7. GetStudentsByLevel
- **Route**: `GET /Student/GetStudentsByLevel/{levelId}`
- **Purpose**: Get students enrolled in specific level
- **Returns**: JSON array for AJAX consumption
- **Used by**: Class management, enrollment processing

#### 8. GetStudentEnrollments
- **Route**: `GET /Student/GetStudentEnrollments/{id}`
- **Purpose**: Get student's enrollment history
- **Returns**: JSON array with enrollment details

#### 9. GetStudentAcademicRecord
- **Route**: `GET /Student/GetStudentAcademicRecord/{id}`
- **Purpose**: Get student's academic performance record
- **Returns**: JSON object with academic data

## Frontend Implementation

### Views Structure
```
Views/Student/
├── Index.cshtml                 # Main students listing page
├── _ListStudents.cshtml         # Partial view for students table
├── _addStudent.cshtml           # Modal form for creating students
├── _editStudent.cshtml          # Modal form for editing students
├── _deleteStudent.cshtml       # Modal confirmation for deletion
└── _detailsStudent.cshtml       # Modal for student details
```

### Key Features

#### 1. Search and Filtering
- **Current Implementation**: Search by student name via SearchText
- **API Capability**: Supports name, level, and status filtering
- **Enhancement Opportunity**: Could implement advanced filtering by enrollment date, academic status

#### 2. Pagination
- Uses `PageList<GetStudentDTO>` for pagination
- Configurable page size (default: 10, options: 20, 50, 100)
- Maintains search filters across pages

#### 3. Status Display
- **Active**: Green badge for active students
- **Inactive**: Gray badge for inactive students
- **Graduated**: Blue badge for graduated students
- **Suspended**: Red badge for suspended students
- **Withdrawn**: Orange badge for withdrawn students

#### 4. Actions Menu
- **Details**: View complete student information
- **Edit**: Available for all students
- **Enrollments**: View student enrollment history
- **Academic Record**: View academic performance
- **Delete**: Available with proper permissions

#### 5. Form Features
- **User Integration**: Link to existing user accounts
- **Student Code**: Unique identifier for each student
- **Personal Information**: Name, date of birth, gender, nationality
- **Contact Details**: Phone, email, address
- **Emergency Contacts**: Required contact information
- **Academic Information**: Current level, enrollment date
- **Status Management**: Academic status tracking
- **Notes**: Additional information about the student

## Service Layer

### StudentService Implementation
```csharp
public class StudentService : BaseApiService, IStudentService
{
    // Implements all CRUD operations
    // Handles HTTP communication with API
    // Manages query parameter construction
    // Provides typed responses
}
```

### Key Methods
- `GetAllStudents()`: Retrieve students with optional filters
- `GetStudent()`: Get specific student by ID
- `GetStudentByUserId()`: Get student by linked user ID
- `GetStudentsByLevel()`: Get students enrolled in specific level
- `GetStudentEnrollments()`: Get student's enrollment history
- `GetStudentAcademicRecord()`: Get student's academic performance
- `CreateStudent()`: Create new student
- `UpdateStudent()`: Update existing student
- `DeleteStudent()`: Delete student by ID

## Business Logic

### 1. Student Creation Workflow
1. User clicks "Add Student" button
2. Modal opens with student creation form
3. User fills required information:
   - User ID (link to existing user account)
   - Student Code (unique identifier)
   - Personal information (name, date of birth, gender)
   - Contact details (phone, email, address)
   - Emergency contact information
   - Academic level assignment
   - Enrollment date
4. System validates business rules
5. Student profile created with specified properties
6. Available for enrollment and academic tracking

### 2. User Integration
- Each student must be linked to a system user account
- User account provides authentication and basic profile information
- Student profile extends user account with academic-specific data
- Enables unified login and profile management

### 3. Academic Status Management
- **Active**: Currently enrolled and attending classes
- **Inactive**: Temporarily not attending classes
- **Graduated**: Successfully completed academic program
- **Suspended**: Temporarily barred from attending classes
- **Withdrawn**: Permanently left the institution

### 4. Emergency Contact Requirements
- **Contact Name**: Full name of emergency contact
- **Phone Number**: Primary contact number
- **Relationship**: Relationship to student (Parent, Guardian, etc.)
- **Required for**: Student safety and communication protocols

### 5. Academic Level Integration
- Students assigned to appropriate academic levels
- Level determines course eligibility and progression
- Integration with enrollment and class management systems

## Validation Rules

### Business Validation
- Student codes must be unique
- User ID must reference existing user account
- Emergency contact information is required
- Academic level must be valid
- Enrollment date must be valid
- Date of birth must be reasonable

### Form Validation
- Required fields enforced
- String length limits
- Date validations
- Email format validation
- Phone number format validation
- Client-side real-time validation
- Server-side validation as backup

## Integration Points

- **User Management**: Integration with user account system
- **Academic Levels**: Integration with level management
- **Enrollment System**: Student enrollment processing
- **Class Management**: Student-class relationships
- **Fee Management**: Student fee assignments
- **Academic Records**: Performance and progress tracking
- **Reporting**: Student analytics and reports

## Performance Considerations

- **Pagination**: Large student catalogs handled efficiently
- **Caching**: Consider caching student data for frequent access
- **Indexing**: Database indexes on frequently queried fields
- **Lazy Loading**: Partial views for better page load times
- **Search Optimization**: Efficient filtering and search

## Security Considerations

- **Authentication**: All endpoints require valid JWT token
- **Authorization**: Role-based permissions enforced
- **Input Validation**: Server-side validation on all DTOs
- **CSRF Protection**: Anti-forgery tokens on all forms
- **Audit Trail**: CreatedBy/ModifiedBy tracking on all changes
- **Data Protection**: Soft delete for student history preservation
- **Privacy Protection**: Sensitive student information properly protected

## Reporting and Analytics

- Student enrollment statistics
- Academic level distribution
- Student status tracking
- Enrollment trends and patterns
- Academic performance metrics
- Demographic analysis

## Future Enhancements

1. **Advanced Search**: Multi-criteria search with all filter options
2. **Student Templates**: Predefined student profiles for quick creation
3. **Bulk Operations**: Mass update student information
4. **Photo Management**: Student photo upload and management
5. **Document Management**: Academic documents and certificates
6. **Parent Portal**: Parent access to student information
7. **Mobile Optimization**: Responsive design improvements
8. **Academic Progress**: Visual progress tracking
9. **Communication Tools**: Integrated messaging system
10. **Performance Analytics**: Advanced academic performance reporting

## Usage Examples

### Creating a New Student
1. Navigate to Students management
2. Click "Add Student" button
3. Fill in student details:
   - User ID: Select from existing users
   - Code: "S001"
   - Name: "John Doe"
   - Date of Birth: "2000-01-15"
   - Academic Level: "Beginner"
   - Emergency Contact: "Jane Doe (Mother)"
4. Set contact information and address
5. Save student profile

### Viewing Student Academic Record
1. Open student details
2. Click "Academic Record" action
3. View enrollment history
4. Check academic performance
5. Review progress and achievements

### Managing Student Status
1. Select student from list
2. Edit student information
3. Update academic status
4. Modify academic level if needed
5. Save changes

## API Response Examples

### GetStudentsByLevel Response
```json
[
  {
    "id": "guid-here",
    "text": "S001 - John Doe",
    "code": "S001",
    "name": "John Doe",
    "email": "john.doe@example.com",
    "academicStatus": "Active",
    "levelName": "Beginner"
  }
]
```

### GetStudent Response
```json
{
  "studentId": "guid-here",
  "userId": "user-guid-here",
  "studentCode": "S001",
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "2000-01-15",
  "gender": "Male",
  "nationality": "American",
  "phoneNumber": "+1234567890",
  "email": "john.doe@example.com",
  "address": "123 Main St, City, State",
  "emergencyContactName": "Jane Doe",
  "emergencyContactPhone": "+1234567891",
  "emergencyContactRelationship": "Mother",
  "academicStatus": "Active",
  "currentLevelId": "level-guid-here",
  "enrollmentDate": "2023-09-01"
}
```

### GetStudentEnrollments Response
```json
[
  {
    "enrollmentId": "guid-here",
    "courseName": "Chinese Level 1",
    "sectionCode": "CHN101-A",
    "enrollmentDate": "2023-09-01",
    "status": "Active",
    "progress": 75
  }
]
```

### GetStudentAcademicRecord Response
```json
{
  "studentId": "guid-here",
  "totalEnrollments": 3,
  "completedCourses": 2,
  "currentCourses": 1,
  "averageGrade": 85.5,
  "attendanceRate": 92.3,
  "lastActivity": "2024-01-15"
}
```

This comprehensive Student management system provides a robust foundation for student administration within the Hong Wen education platform.
