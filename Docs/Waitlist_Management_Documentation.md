# Waitlist Management System Documentation

## Overview
The Waitlist Management System handles student waitlist management within the Hong Wen education platform. It provides comprehensive functionality for managing course waitlists, student priority tracking, automatic promotion from waitlist to enrollment, and waitlist reordering based on priority and enrollment date.

## Business Workflow

### 1. Waitlist Lifecycle
```
Course Full → Student Added to Waitlist → Priority Management → Automatic Promotion → Enrollment Confirmation
```

### 2. Key Business Rules
- **Waitlist Codes**: Must be unique across the system (e.g., "WL001", "WAIT001")
- **Student Integration**: Each waitlist entry must be linked to an existing student
- **Course Integration**: Each waitlist entry must be linked to an existing course section
- **Priority Management**: Students ordered by priority and enrollment date
- **Automatic Promotion**: Students automatically promoted when spots become available
- **Status Tracking**: Pending, Active, Promoted, Cancelled, Expired status management
- **Capacity Management**: Waitlist capacity limits enforced
- **Notification System**: Automatic notifications for waitlist status changes

### 3. User Roles and Permissions
- **ViewWaitlist**: Can view waitlist information and student positions
- **ManageWaitlist**: Can create and edit waitlist entries, manage priorities
- **DeleteWaitlist**: Can delete waitlist entries (separate permission for data protection)

## API Endpoints

### Base URL: `/sms/Waitlist`

| Method | Endpoint | Description | Parameters | Permission |
|--------|----------|-------------|------------|------------|
| GET | `/` | Get all waitlist entries | `studentId`, `sectionId`, `status` | ViewWaitlist |
| GET | `/{waitlistId}` | Get specific waitlist entry | `waitlistId` (GUID) | ViewWaitlist |
| GET | `/by-student/{studentId}` | Get waitlist entries by student | `studentId` (GUID) | ViewWaitlist |
| GET | `/by-section/{sectionId}` | Get waitlist entries by section | `sectionId` (GUID) | ViewWaitlist |
| GET | `/by-status/{status}` | Get waitlist entries by status | `status` (string) | ViewWaitlist |
| GET | `/{waitlistId}/position` | Get waitlist position | `waitlistId` (GUID) | ViewWaitlist |
| POST | `/` | Create new waitlist entry | CreateWaitlistDTO in body | ManageWaitlist |
| PUT | `/` | Update existing waitlist entry | UpdateWaitlistDTO in body | ManageWaitlist |
| PUT | `/reorder` | Reorder waitlist priorities | ReorderWaitlistDTO in body | ManageWaitlist |
| PUT | `/{waitlistId}/promote` | Promote from waitlist | PromoteFromWaitlistDTO in body | ManageWaitlist |
| DELETE | `/{waitlistId}` | Delete waitlist entry | `waitlistId` (GUID) | DeleteWaitlist |

### Query Parameters
- **studentId**: Filter by student (GUID)
- **sectionId**: Filter by course section (GUID)
- **status**: Filter by waitlist status (Pending, Active, Promoted, etc.)

## Data Transfer Objects (DTOs)

### WaitlistBaseDTO (Abstract)
```csharp
public abstract record class WaitlistBaseDTO
{
    [Required]
    public Guid StudentId { get; set; }
    
    [Required]
    public Guid SectionId { get; set; }
    
    [Required, StringLength(20)]
    public string WaitlistCode { get; set; } = string.Empty;
    
    [Required]
    public DateTime WaitlistDate { get; set; }
    
    [Range(1, int.MaxValue)]
    public int PriorityOrder { get; set; } = 1;
    
    [StringLength(20)]
    public string Status { get; set; } = "Pending";
    
    public DateTime? ExpectedStartDate { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    [StringLength(500)]
    public string? Notes { get; set; }
    
    [StringLength(100)]
    public string? SpecialRequests { get; set; }
    
    public bool IsPriorityStudent { get; set; } = false;
    
    [StringLength(50)]
    public string? PriorityReason { get; set; }
}
```

### CreateWaitlistDTO
- Inherits from WaitlistBaseDTO
- Additional: `CreatedBy` (auto-populated from JWT claims)

### UpdateWaitlistDTO
- Inherits from WaitlistBaseDTO
- Additional: `WaitlistId`, `ModifiedBy` (auto-populated from JWT claims)

### GetWaitlistDTO
- Inherits from WaitlistBaseDTO
- Additional: `WaitlistId`, `CreatedBy`, `ModifiedBy`, `CreateDate`, `UpdateDate`
- Display fields: `StudentName`, `StudentCode`, `CourseName`, `CourseCode`, `SectionName`, `SectionCode`, `TermName`, `TeacherName`, `ClassroomName`

### ReorderWaitlistDTO
```csharp
public record class ReorderWaitlistDTO
{
    [Required]
    public Guid SectionId { get; set; }
    
    [Required]
    public List<WaitlistOrderItem> WaitlistItems { get; set; } = new();
    
    public string? ModifiedBy { get; set; }
}

public record class WaitlistOrderItem
{
    [Required]
    public Guid WaitlistId { get; set; }
    
    [Required]
    public int NewPriorityOrder { get; set; }
}
```

### PromoteFromWaitlistDTO
```csharp
public record class PromoteFromWaitlistDTO
{
    [Required]
    public Guid WaitlistId { get; set; }
    
    [Required]
    public Guid StudentId { get; set; }
    
    [Required]
    public Guid SectionId { get; set; }
    
    public Guid? EnrollmentId { get; set; }
    
    [StringLength(500)]
    public string? PromotionNotes { get; set; }
    
    public string? PromotedBy { get; set; }
}
```

## Application Layer Usage

### Controller Actions

#### 1. Index (Main View)
- **Route**: `GET /Waitlist`
- **Purpose**: Display paginated list of all waitlist entries
- **Features**: Search functionality, pagination, waitlist creation
- **Permissions**: ViewWaitlist

#### 2. ListWaitlist (AJAX Partial View)
- **Route**: `GET /Waitlist/ListWaitlist`
- **Purpose**: Refresh waitlist list with search filters
- **Parameters**: `ListWaitlistDTOs` (includes SearchText, Page, PageSize)
- **Returns**: Partial view with filtered results

#### 3. FetchWaitlist (API for JavaScript)
- **Route**: `GET /Waitlist/FetchWaitlist`
- **Purpose**: Get waitlist data for JavaScript/AJAX calls
- **Parameters**: `i` (waitlist filter)
- **Returns**: JSON array of waitlist objects

#### 4. AddWaitlist
- **GET Route**: `GET /Waitlist/AddWaitlist` - Show create form
- **POST Route**: `POST /Waitlist/AddWaitlist` - Process creation
- **Model**: `CreateWaitlistDTO`
- **Features**: Student selection, course assignment, priority management
- **Permissions**: ManageWaitlist

#### 5. EditWaitlist
- **GET Route**: `GET /Waitlist/EditWaitlist/{id}` - Show edit form
- **POST Route**: `POST /Waitlist/EditWaitlist` - Process update
- **Model**: `UpdateWaitlistDTO`
- **Features**: Pre-populated form, priority management
- **Permissions**: ManageWaitlist

#### 6. DeleteWaitlist
- **GET Route**: `GET /Waitlist/DeleteWaitlist/{id}` - Show confirmation
- **POST Route**: `POST /Waitlist/DeleteWaitlistConfirmed` - Process deletion
- **Permissions**: DeleteWaitlist

#### 7. PromoteFromWaitlist
- **GET Route**: `GET /Waitlist/PromoteFromWaitlist/{id}` - Show promotion form
- **POST Route**: `POST /Waitlist/PromoteFromWaitlist` - Process promotion
- **Model**: `PromoteFromWaitlistDTO`
- **Features**: Automatic enrollment creation, waitlist removal
- **Permissions**: ManageWaitlist

#### 8. ReorderWaitlist
- **GET Route**: `GET /Waitlist/ReorderWaitlist/{sectionId}` - Show reorder form
- **POST Route**: `POST /Waitlist/ReorderWaitlist` - Process reordering
- **Model**: `ReorderWaitlistDTO`
- **Features**: Drag-and-drop priority management
- **Permissions**: ManageWaitlist

#### 9. GetWaitlistBySection
- **Route**: `GET /Waitlist/GetWaitlistBySection/{sectionId}`
- **Purpose**: Get waitlist entries for specific course section
- **Returns**: JSON array for AJAX consumption
- **Used by**: Course management, capacity tracking

#### 10. GetWaitlistByStudent
- **Route**: `GET /Waitlist/GetWaitlistByStudent/{studentId}`
- **Purpose**: Get waitlist entries for specific student
- **Returns**: JSON array with waitlist details
- **Used by**: Student management, enrollment tracking

#### 11. GetWaitlistPosition
- **Route**: `GET /Waitlist/GetWaitlistPosition/{id}`
- **Purpose**: Get current position in waitlist
- **Returns**: JSON object with position data

## Frontend Implementation

### Views Structure
```
Views/Waitlist/
├── Index.cshtml                 # Main waitlist listing page
├── _ListWaitlists.cshtml        # Partial view for waitlist table
├── _addWaitlist.cshtml          # Modal form for creating waitlist entries
├── _editWaitlist.cshtml         # Modal form for editing waitlist entries
├── _deleteWaitlist.cshtml       # Modal confirmation for deletion
├── _detailsWaitlist.cshtml      # Modal for waitlist details
├── _promoteFromWaitlist.cshtml  # Modal form for promoting from waitlist
└── _reorderWaitlist.cshtml      # Modal form for reordering waitlist
```

### Key Features

#### 1. Search and Filtering
- **Current Implementation**: Search by student name, course name via SearchText
- **API Capability**: Supports student, section, and status filtering
- **Enhancement Opportunity**: Could implement advanced filtering by waitlist date, priority

#### 2. Pagination
- Uses `PageList<GetWaitlistDTO>` for pagination
- Configurable page size (default: 10, options: 20, 50, 100)
- Maintains search filters across pages

#### 3. Status Display
- **Pending**: Yellow badge for pending waitlist entries
- **Active**: Green badge for active waitlist entries
- **Promoted**: Blue badge for promoted entries
- **Cancelled**: Red badge for cancelled entries
- **Expired**: Gray badge for expired entries

#### 4. Actions Menu
- **Details**: View complete waitlist information
- **Edit**: Available for all waitlist entries
- **Promote**: Promote student from waitlist to enrollment
- **Reorder**: Reorder waitlist priorities
- **Delete**: Available with proper permissions

#### 5. Form Features
- **Student Selection**: Link to existing students
- **Course Assignment**: Select from available course sections
- **Waitlist Code**: Unique identifier for each waitlist entry
- **Priority Management**: Set priority order for waitlist
- **Date Management**: Waitlist date, expected start date, expiry date
- **Status Tracking**: Waitlist status management
- **Special Requests**: Additional requirements or preferences
- **Priority Student**: Mark priority students with reasons
- **Notes**: Additional information about the waitlist entry

#### 6. Priority Management
- **Drag-and-Drop**: Visual priority reordering
- **Priority Order**: Numeric priority assignment
- **Priority Students**: Special priority for certain students
- **Priority Reasons**: Documentation for priority assignments

#### 7. Promotion Management
- **Automatic Promotion**: Promote students when spots become available
- **Manual Promotion**: Manual promotion with notes
- **Enrollment Creation**: Automatic enrollment creation upon promotion
- **Notification System**: Notify students of promotion

## Service Layer

### WaitlistService Implementation
```csharp
public class WaitlistService : BaseApiService, IWaitlistService
{
    // Implements all CRUD operations
    // Handles HTTP communication with API
    // Manages query parameter construction
    // Provides typed responses
    // Supports business operations
}
```

### Key Methods
- `GetAllWaitlists()`: Retrieve waitlist entries with optional filters
- `GetWaitlist()`: Get specific waitlist entry by ID
- `GetWaitlistBySection()`: Get waitlist entries for specific section
- `GetWaitlistByStudent()`: Get waitlist entries for specific student
- `GetWaitlistPosition()`: Get current position in waitlist
- `CreateWaitlist()`: Create new waitlist entry
- `UpdateWaitlist()`: Update existing waitlist entry
- `PromoteFromWaitlist()`: Promote student from waitlist
- `ReorderWaitlist()`: Reorder waitlist priorities
- `DeleteWaitlist()`: Delete waitlist entry by ID

## Business Logic

### 1. Waitlist Creation Workflow
1. User clicks "Add Waitlist" button
2. Modal opens with waitlist creation form
3. User fills required information:
   - Student selection (from existing students)
   - Course section assignment (from available sections)
   - Waitlist code (unique identifier)
   - Waitlist date
   - Priority order
   - Expected start date
   - Special requests or notes
4. System validates business rules:
   - Student exists and is active
   - Course section exists and is available
   - Student not already enrolled in the course
   - Student not already on waitlist for the course
   - Priority order is valid
5. Waitlist entry created with specified properties
6. Student is added to the waitlist

### 2. Priority Management
- **Priority Order**: Students ordered by priority and waitlist date
- **Priority Students**: Special priority for certain students
- **Priority Reasons**: Documentation for priority assignments
- **Automatic Ordering**: System maintains priority order
- **Manual Reordering**: Administrators can manually reorder priorities

### 3. Capacity Management
- **Waitlist Capacity**: Maximum number of students on waitlist
- **Course Capacity**: Maximum number of students per course section
- **Waitlist Limits**: Prevent excessive waitlist entries
- **Capacity Tracking**: Real-time capacity monitoring

### 4. Promotion Management
- **Automatic Promotion**: Students automatically promoted when spots become available
- **Manual Promotion**: Administrators can manually promote students
- **Enrollment Creation**: Automatic enrollment creation upon promotion
- **Notification System**: Notify students of promotion status
- **Priority-Based**: Promotion based on priority order

### 5. Status Management
- **Pending**: Waitlist entry submitted but not yet processed
- **Active**: Student is actively on the waitlist
- **Promoted**: Student has been promoted to enrollment
- **Cancelled**: Student voluntarily removed from waitlist
- **Expired**: Waitlist entry has expired

### 6. Notification System
- **Promotion Notifications**: Notify students when promoted
- **Status Updates**: Notify students of waitlist status changes
- **Expiry Notifications**: Notify students of waitlist expiry
- **Priority Changes**: Notify students of priority changes

## Validation Rules

### Business Validation
- Student must exist and be active
- Course section must exist and be available
- Student must not already be enrolled in the course
- Student must not already be on waitlist for the course
- Waitlist code must be unique
- Priority order must be valid
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
- **Enrollment Management**: Integration with enrollment system
- **Teacher Management**: Integration with teacher assignments
- **Classroom Management**: Integration with classroom assignments
- **Notification System**: Integration with notification services
- **Reporting**: Waitlist analytics and reports

## Performance Considerations

- **Pagination**: Large waitlist catalogs handled efficiently
- **Caching**: Consider caching waitlist data for frequent access
- **Indexing**: Database indexes on frequently queried fields
- **Lazy Loading**: Partial views for better page load times
- **Search Optimization**: Efficient filtering and search
- **Priority Updates**: Efficient priority reordering

## Security Considerations

- **Authentication**: All endpoints require valid JWT token
- **Authorization**: Role-based permissions enforced
- **Input Validation**: Server-side validation on all DTOs
- **CSRF Protection**: Anti-forgery tokens on all forms
- **Audit Trail**: CreatedBy/ModifiedBy tracking on all changes
- **Data Protection**: Soft delete for waitlist history preservation
- **Privacy Protection**: Sensitive waitlist information properly protected

## Reporting and Analytics

- Waitlist statistics by course and section
- Student waitlist patterns
- Waitlist capacity utilization
- Promotion rates and timing
- Priority distribution analysis
- Waitlist performance metrics

## Future Enhancements

1. **Advanced Priority Management**: Complex priority rules and algorithms
2. **Automated Promotion**: Fully automated promotion system
3. **Waitlist Analytics**: Advanced waitlist analytics and reporting
4. **Mobile Notifications**: Mobile push notifications for waitlist updates
5. **Integration APIs**: External system integration capabilities
6. **Real-time Updates**: Live waitlist status updates
7. **Bulk Operations**: Mass waitlist operations
8. **Waitlist Templates**: Predefined waitlist configurations
9. **Advanced Filtering**: Multi-criteria search and filtering
10. **Performance Optimization**: Enhanced performance and scalability

## Usage Examples

### Creating a Waitlist Entry
1. Navigate to Waitlist management
2. Click "Add Waitlist" button
3. Fill in waitlist details:
   - Student: Select from existing students
   - Course Section: Select from available sections
   - Waitlist Code: "WL001"
   - Priority Order: 1
   - Expected Start Date: Set appropriate date
   - Special Requests: Any special requirements
4. System validates student eligibility
5. Save waitlist entry

### Managing Waitlist Priorities
1. Select course section from list
2. Click "Reorder" action
3. Drag and drop students to reorder priorities
4. Update priority order
5. Save changes

### Promoting from Waitlist
1. Select waitlist entry from list
2. Click "Promote" action
3. Fill in promotion details:
   - Promotion Notes: Reason for promotion
   - Enrollment Details: Confirm enrollment information
4. System creates enrollment automatically
5. Student is removed from waitlist

### Viewing Waitlist Position
1. Open waitlist details
2. Click "Position" action
3. View current position in waitlist
4. Check estimated promotion time
5. Review priority information

## API Response Examples

### GetWaitlistBySection Response
```json
[
  {
    "id": "guid-here",
    "text": "WL001 - John Doe",
    "waitlistCode": "WL001",
    "studentName": "John Doe",
    "studentCode": "S001",
    "priorityOrder": 1,
    "status": "Active",
    "waitlistDate": "2023-09-01"
  }
]
```

### GetWaitlist Response
```json
{
  "waitlistId": "guid-here",
  "studentId": "student-guid-here",
  "sectionId": "section-guid-here",
  "waitlistCode": "WL001",
  "studentName": "John Doe",
  "studentCode": "S001",
  "courseName": "Chinese Level 1",
  "courseCode": "CHN101",
  "sectionName": "Chinese Level 1 - Section A",
  "sectionCode": "CHN101-A",
  "termName": "Fall 2023",
  "teacherName": "Jane Smith",
  "classroomName": "A101",
  "waitlistDate": "2023-09-01",
  "priorityOrder": 1,
  "status": "Active",
  "expectedStartDate": "2023-09-15",
  "expiryDate": "2023-12-31",
  "notes": "Student prefers morning classes",
  "specialRequests": "Need accessible classroom",
  "isPriorityStudent": false,
  "priorityReason": null,
  "createdBy": "admin",
  "createDate": "2023-09-01T00:00:00Z"
}
```

### GetWaitlistPosition Response
```json
{
  "waitlistId": "guid-here",
  "currentPosition": 3,
  "totalWaitlistSize": 15,
  "estimatedPromotionDate": "2023-10-15",
  "priorityOrder": 1,
  "waitlistDate": "2023-09-01",
  "status": "Active"
}
```

### ReorderWaitlist Response
```json
{
  "sectionId": "section-guid-here",
  "waitlistItems": [
    {
      "waitlistId": "waitlist-guid-1",
      "newPriorityOrder": 1
    },
    {
      "waitlistId": "waitlist-guid-2",
      "newPriorityOrder": 2
    }
  ],
  "modifiedBy": "admin"
}
```

### PromoteFromWaitlist Response
```json
{
  "waitlistId": "guid-here",
  "studentId": "student-guid-here",
  "sectionId": "section-guid-here",
  "enrollmentId": "enrollment-guid-here",
  "promotionNotes": "Student promoted due to priority status",
  "promotedBy": "admin",
  "promotionDate": "2023-09-15T00:00:00Z"
}
```

This comprehensive Waitlist management system provides a robust foundation for waitlist administration within the Hong Wen education platform, supporting all aspects of waitlist management from entry creation to promotion and priority management.
