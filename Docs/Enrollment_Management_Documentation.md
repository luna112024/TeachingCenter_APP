# Enrollment Management System Documentation

## Overview
The Enrollment Management System handles student course enrollments within the Hong Wen education platform. It provides comprehensive functionality for managing student enrollments, course assignments, enrollment status tracking, and academic progress monitoring.

## Business Workflow

### 1. Enrollment Lifecycle
```
Course Selection → Enrollment Application → Approval Process → Active Enrollment → Progress Tracking → Completion/Graduation
```

### 2. Key Business Rules
- **Enrollment Codes**: Must be unique across the system (e.g., "ENR001", "ENROLL001")
- **Student Integration**: Each enrollment must be linked to an existing student
- **Course Integration**: Each enrollment must be linked to an existing course section
- **Term Integration**: Each enrollment must be linked to an active term
- **Enrollment Status**: Pending, Active, Completed, Dropped, Suspended tracking
- **Capacity Management**: Course section capacity limits enforced
- **Prerequisites**: Course prerequisite requirements validated
- **Academic Progress**: Enrollment progress and completion tracking

### 3. User Roles and Permissions
- **ViewEnrollments**: Can view enrollment information and academic records
- **ManageEnrollments**: Can create and edit enrollments
- **DeleteEnrollments**: Can delete enrollments (separate permission for data protection)

## API Endpoints

### Base URL: `/sms/Enrollment`

| Method | Endpoint | Description | Parameters | Permission |
|--------|----------|-------------|------------|------------|
| GET | `/` | Get all enrollments | `studentId`, `courseId`, `termId`, `status` | ViewEnrollments |
| GET | `/{enrollmentId}` | Get specific enrollment | `enrollmentId` (GUID) | ViewEnrollments |
| GET | `/by-student/{studentId}` | Get enrollments by student | `studentId` (GUID) | ViewEnrollments |
| GET | `/by-course/{courseId}` | Get enrollments by course | `courseId` (GUID) | ViewEnrollments |
| GET | `/by-term/{termId}` | Get enrollments by term | `termId` (GUID) | ViewEnrollments |
| GET | `/by-status/{status}` | Get enrollments by status | `status` (string) | ViewEnrollments |
| GET | `/{enrollmentId}/progress` | Get enrollment progress | `enrollmentId` (GUID) | ViewEnrollments |
| GET | `/{enrollmentId}/attendance` | Get attendance record | `enrollmentId` (GUID) | ViewEnrollments |
| POST | `/` | Create new enrollment | CreateEnrollmentDTO in body | ManageEnrollments |
| PUT | `/` | Update existing enrollment | UpdateEnrollmentDTO in body | ManageEnrollments |
| DELETE | `/{enrollmentId}` | Delete enrollment | `enrollmentId` (GUID) | DeleteEnrollments |

### Query Parameters
- **studentId**: Filter by student (GUID)
- **courseId**: Filter by course (GUID)
- **termId**: Filter by term (GUID)
- **status**: Filter by enrollment status (Active, Completed, Dropped, etc.)

## Data Transfer Objects (DTOs)

### EnrollmentBaseDTO (Abstract)
```csharp
public abstract record class EnrollmentBaseDTO
{
    [Required]
    public Guid StudentId { get; set; }
    
    [Required]
    public Guid CourseId { get; set; }
    
    [Required]
    public Guid TermId { get; set; }
    
    [Required]
    public Guid ClassSectionId { get; set; }
    
    [Required, StringLength(20)]
    public string EnrollmentCode { get; set; } = string.Empty;
    
    [Required]
    public DateTime EnrollmentDate { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    [StringLength(20)]
    public string Status { get; set; } = "Pending";
    
    [Range(0, 100)]
    public decimal? ProgressPercentage { get; set; }
    
    [StringLength(5)]
    public string? FinalGrade { get; set; }
    
    [StringLength(20)]
    public string? AttendanceStatus { get; set; }
    
    [Range(0, int.MaxValue)]
    public int? AttendanceCount { get; set; }
    
    [Range(0, int.MaxValue)]
    public int? TotalClasses { get; set; }
    
    public decimal? TuitionAmount { get; set; }
    
    public decimal? PaidAmount { get; set; }
    
    public decimal? OutstandingAmount { get; set; }
    
    public string? Notes { get; set; }
}
```

### CreateEnrollmentDTO
- Inherits from EnrollmentBaseDTO
- Additional: `CreatedBy` (auto-populated from JWT claims)

### UpdateEnrollmentDTO
- Inherits from EnrollmentBaseDTO
- Additional: `EnrollmentId`, `ModifiedBy` (auto-populated from JWT claims)

### GetEnrollmentDTO
- Inherits from EnrollmentBaseDTO
- Additional: `EnrollmentId`, `CreatedBy`, `ModifiedBy`, `CreateDate`, `UpdateDate`
- Display fields: `StudentName`, `StudentCode`, `CourseName`, `CourseCode`, `TermName`, `SectionCode`, `TeacherName`, `ClassroomName`

## Application Layer Usage

### Controller Actions

#### 1. Index (Main View)
- **Route**: `GET /Enrollment`
- **Purpose**: Display paginated list of all enrollments
- **Features**: Search functionality, pagination, enrollment creation
- **Permissions**: ViewEnrollments

#### 2. ListEnrollment (AJAX Partial View)
- **Route**: `GET /Enrollment/ListEnrollment`
- **Purpose**: Refresh enrollment list with search filters
- **Parameters**: `ListEnrollmentDTOs` (includes SearchText, Page, PageSize)
- **Returns**: Partial view with filtered results

#### 3. FetchEnrollment (API for JavaScript)
- **Route**: `GET /Enrollment/FetchEnrollment`
- **Purpose**: Get enrollment data for JavaScript/AJAX calls
- **Parameters**: `i` (enrollment filter)
- **Returns**: JSON array of enrollment objects

#### 4. AddEnrollment
- **GET Route**: `GET /Enrollment/AddEnrollment` - Show create form
- **POST Route**: `POST /Enrollment/AddEnrollment` - Process creation
- **Model**: `CreateEnrollmentDTO`
- **Features**: Student selection, course assignment, capacity validation
- **Permissions**: ManageEnrollments

#### 5. EditEnrollment
- **GET Route**: `GET /Enrollment/EditEnrollment/{id}` - Show edit form
- **POST Route**: `POST /Enrollment/EditEnrollment` - Process update
- **Model**: `UpdateEnrollmentDTO`
- **Features**: Pre-populated form, status management
- **Permissions**: ManageEnrollments

#### 6. DeleteEnrollment
- **GET Route**: `GET /Enrollment/DeleteEnrollment/{id}` - Show confirmation
- **POST Route**: `POST /Enrollment/DeleteEnrollmentConfirmed` - Process deletion
- **Permissions**: DeleteEnrollments

#### 7. GetEnrollmentsByStudent
- **Route**: `GET /Enrollment/GetEnrollmentsByStudent/{studentId}`
- **Purpose**: Get enrollments for specific student
- **Returns**: JSON array for AJAX consumption
- **Used by**: Student management, academic records

#### 8. GetEnrollmentsByCourse
- **Route**: `GET /Enrollment/GetEnrollmentsByCourse/{courseId}`
- **Purpose**: Get enrollments for specific course
- **Returns**: JSON array with enrollment details
- **Used by**: Course management, capacity tracking

#### 9. GetEnrollmentProgress
- **Route**: `GET /Enrollment/GetEnrollmentProgress/{id}`
- **Purpose**: Get enrollment progress and performance
- **Returns**: JSON object with progress data

#### 10. GetEnrollmentAttendance
- **Route**: `GET /Enrollment/GetEnrollmentAttendance/{id}`
- **Purpose**: Get attendance record for enrollment
- **Returns**: JSON object with attendance data

## Frontend Implementation

### Views Structure
```
Views/Enrollment/
├── Index.cshtml                 # Main enrollments listing page
├── _ListEnrollments.cshtml     # Partial view for enrollments table
├── _addEnrollment.cshtml        # Modal form for creating enrollments
├── _editEnrollment.cshtml       # Modal form for editing enrollments
├── _deleteEnrollment.cshtml     # Modal confirmation for deletion
└── _detailsEnrollment.cshtml    # Modal for enrollment details
```

### Key Features

#### 1. Search and Filtering
- **Current Implementation**: Search by student name, course name via SearchText
- **API Capability**: Supports student, course, term, and status filtering
- **Enhancement Opportunity**: Could implement advanced filtering by enrollment date, progress

#### 2. Pagination
- Uses `PageList<GetEnrollmentDTO>` for pagination
- Configurable page size (default: 10, options: 20, 50, 100)
- Maintains search filters across pages

#### 3. Status Display
- **Pending**: Yellow badge for pending enrollments
- **Active**: Green badge for active enrollments
- **Completed**: Blue badge for completed enrollments
- **Dropped**: Red badge for dropped enrollments
- **Suspended**: Orange badge for suspended enrollments

#### 4. Actions Menu
- **Details**: View complete enrollment information
- **Edit**: Available for all enrollments
- **Progress**: View enrollment progress and performance
- **Attendance**: View attendance record
- **Delete**: Available with proper permissions

#### 5. Form Features
- **Student Selection**: Link to existing students
- **Course Assignment**: Select from available courses
- **Term Integration**: Link to active terms
- **Class Section**: Assign to specific class sections
- **Enrollment Code**: Unique identifier for each enrollment
- **Date Management**: Enrollment, start, and end dates
- **Status Tracking**: Enrollment status management
- **Progress Monitoring**: Progress percentage and grades
- **Attendance Tracking**: Attendance status and counts
- **Financial Information**: Tuition and payment tracking
- **Notes**: Additional information about the enrollment

## Service Layer

### EnrollmentService Implementation
```csharp
public class EnrollmentService : BaseApiService, IEnrollmentService
{
    // Implements all CRUD operations
    // Handles HTTP communication with API
    // Manages query parameter construction
    // Provides typed responses
}
```

### Key Methods
- `GetAllEnrollments()`: Retrieve enrollments with optional filters
- `GetEnrollment()`: Get specific enrollment by ID
- `GetEnrollmentsByStudent()`: Get enrollments for specific student
- `GetEnrollmentsByCourse()`: Get enrollments for specific course
- `GetEnrollmentsByTerm()`: Get enrollments for specific term
- `GetEnrollmentProgress()`: Get enrollment progress and performance
- `GetEnrollmentAttendance()`: Get attendance record
- `CreateEnrollment()`: Create new enrollment
- `UpdateEnrollment()`: Update existing enrollment
- `DeleteEnrollment()`: Delete enrollment by ID

## Business Logic

### 1. Enrollment Creation Workflow
1. User clicks "Add Enrollment" button
2. Modal opens with enrollment creation form
3. User fills required information:
   - Student selection (from existing students)
   - Course assignment (from available courses)
   - Term selection (from active terms)
   - Class section assignment
   - Enrollment code (unique identifier)
   - Enrollment date
   - Start and end dates
4. System validates business rules:
   - Student exists and is active
   - Course is available and not full
   - Term is active
   - Class section has capacity
   - Prerequisites are met
5. Enrollment created with specified properties
6. Student is enrolled in the course

### 2. Capacity Management
- **Course Capacity**: Maximum number of students per course section
- **Enrollment Limits**: Prevent over-enrollment
- **Waitlist Integration**: Automatic waitlist management when full
- **Capacity Tracking**: Real-time capacity monitoring

### 3. Prerequisites Validation
- **Course Prerequisites**: Required courses that must be completed
- **Level Requirements**: Minimum academic level requirements
- **Skill Prerequisites**: Required skills or qualifications
- **Automatic Validation**: System checks prerequisites before enrollment

### 4. Status Management
- **Pending**: Enrollment submitted but not yet approved
- **Active**: Student is actively enrolled and attending
- **Completed**: Student successfully completed the course
- **Dropped**: Student voluntarily withdrew from the course
- **Suspended**: Student temporarily suspended from the course

### 5. Progress Tracking
- **Progress Percentage**: Completion percentage of the course
- **Grade Management**: Final grade assignment
- **Attendance Tracking**: Attendance status and counts
- **Performance Monitoring**: Academic performance metrics

## Validation Rules

### Business Validation
- Student must exist and be active
- Course must be available and not full
- Term must be active
- Class section must have capacity
- Prerequisites must be met
- Enrollment code must be unique
- Dates must be valid and logical

### Form Validation
- Required fields enforced
- String length limits
- Date validations
- Numeric range validations
- Client-side real-time validation
- Server-side validation as backup

## Integration Points

- **Student Management**: Integration with student profiles
- **Course Management**: Integration with course and section management
- **Term Management**: Integration with academic terms
- **Teacher Management**: Integration with teacher assignments
- **Classroom Management**: Integration with classroom assignments
- **Fee Management**: Integration with student fee assignments
- **Academic Records**: Integration with performance tracking
- **Reporting**: Enrollment analytics and reports

## Performance Considerations

- **Pagination**: Large enrollment catalogs handled efficiently
- **Caching**: Consider caching enrollment data for frequent access
- **Indexing**: Database indexes on frequently queried fields
- **Lazy Loading**: Partial views for better page load times
- **Search Optimization**: Efficient filtering and search

## Security Considerations

- **Authentication**: All endpoints require valid JWT token
- **Authorization**: Role-based permissions enforced
- **Input Validation**: Server-side validation on all DTOs
- **CSRF Protection**: Anti-forgery tokens on all forms
- **Audit Trail**: CreatedBy/ModifiedBy tracking on all changes
- **Data Protection**: Soft delete for enrollment history preservation
- **Privacy Protection**: Sensitive enrollment information properly protected

## Reporting and Analytics

- Enrollment statistics by course and term
- Student enrollment patterns
- Course capacity utilization
- Enrollment completion rates
- Attendance analytics
- Academic performance metrics

## Future Enhancements

1. **Advanced Search**: Multi-criteria search with all filter options
2. **Bulk Enrollment**: Mass enrollment operations
3. **Enrollment Templates**: Predefined enrollment configurations
4. **Automated Prerequisites**: Automatic prerequisite checking
5. **Waitlist Management**: Integrated waitlist system
6. **Progress Visualization**: Visual progress tracking
7. **Mobile Optimization**: Responsive design improvements
8. **Real-time Updates**: Live enrollment status updates
9. **Integration APIs**: External system integration capabilities
10. **Advanced Analytics**: Comprehensive enrollment analytics

## Usage Examples

### Creating a New Enrollment
1. Navigate to Enrollments management
2. Click "Add Enrollment" button
3. Fill in enrollment details:
   - Student: Select from existing students
   - Course: Select from available courses
   - Term: Select active term
   - Class Section: Assign to specific section
   - Enrollment Code: "ENR001"
   - Dates: Set enrollment and course dates
4. System validates capacity and prerequisites
5. Save enrollment

### Managing Enrollment Status
1. Select enrollment from list
2. Edit enrollment information
3. Update enrollment status
4. Modify progress and grades
5. Update attendance information
6. Save changes

### Viewing Enrollment Progress
1. Open enrollment details
2. Click "Progress" action
3. View completion percentage
4. Check grades and performance
5. Review attendance record

## API Response Examples

### GetEnrollmentsByStudent Response
```json
[
  {
    "id": "guid-here",
    "text": "ENR001 - Chinese Level 1",
    "enrollmentCode": "ENR001",
    "courseName": "Chinese Level 1",
    "sectionCode": "CHN101-A",
    "status": "Active",
    "progressPercentage": 75
  }
]
```

### GetEnrollment Response
```json
{
  "enrollmentId": "guid-here",
  "studentId": "student-guid-here",
  "courseId": "course-guid-here",
  "termId": "term-guid-here",
  "classSectionId": "section-guid-here",
  "enrollmentCode": "ENR001",
  "studentName": "John Doe",
  "studentCode": "S001",
  "courseName": "Chinese Level 1",
  "courseCode": "CHN101",
  "termName": "Fall 2023",
  "sectionCode": "CHN101-A",
  "teacherName": "Jane Smith",
  "classroomName": "A101",
  "enrollmentDate": "2023-09-01",
  "startDate": "2023-09-01",
  "endDate": "2023-12-15",
  "status": "Active",
  "progressPercentage": 75,
  "finalGrade": "A",
  "attendanceStatus": "Good",
  "attendanceCount": 18,
  "totalClasses": 20,
  "tuitionAmount": 500.00,
  "paidAmount": 500.00,
  "outstandingAmount": 0.00
}
```

### GetEnrollmentProgress Response
```json
{
  "enrollmentId": "guid-here",
  "progressPercentage": 75,
  "completedAssignments": 15,
  "totalAssignments": 20,
  "averageGrade": 85.5,
  "attendanceRate": 90.0,
  "lastActivity": "2024-01-15",
  "estimatedCompletion": "2024-02-15"
}
```

### GetEnrollmentAttendance Response
```json
{
  "enrollmentId": "guid-here",
  "attendanceCount": 18,
  "totalClasses": 20,
  "attendanceRate": 90.0,
  "attendanceStatus": "Good",
  "lastAttendance": "2024-01-15",
  "attendanceHistory": [
    {
      "date": "2024-01-15",
      "status": "Present",
      "notes": "On time"
    }
  ]
}
```

This comprehensive Enrollment management system provides a robust foundation for student enrollment administration within the Hong Wen education platform.
