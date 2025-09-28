# Classrooms Management System Documentation

## Overview
The Classrooms Management System handles physical classroom resources within the Hong Wen education platform. It provides comprehensive functionality for managing classroom facilities, equipment, scheduling, and availability tracking.

## Business Workflow

### 1. Classroom Lifecycle
```
Creation → Available → Scheduled → Occupied → Available/Maintenance
```

### 2. Key Business Rules
- **Room Codes**: Must be unique across the system (e.g., "A101", "LAB1", "B205")
- **Capacity Management**: Each classroom has a defined seating capacity
- **Status Tracking**: Real-time status updates (Available, Occupied, Maintenance, Reserved)
- **Building Organization**: Classrooms organized by building and floor level
- **Equipment Tracking**: Detailed equipment and facility management
- **Scheduling Integration**: Integration with class scheduling system

### 3. User Roles and Permissions
- **ViewClassroom**: Can view classroom information and schedules
- **ManageClassroom**: Can create and edit classroom details
- **DeleteClassroom**: Can delete classrooms (separate permission for data protection)

## API Endpoints

### Base URL: `/sms/Classroom`

| Method | Endpoint | Description | Parameters | Permission |
|--------|----------|-------------|------------|------------|
| GET | `/` | Get all classrooms | `status`, `minCapacity`, `name` | ViewClassroom |
| GET | `/{classroomId}` | Get specific classroom | `classroomId` (GUID) | ViewClassroom |
| GET | `/available` | Get available classrooms | `name`, `startDate`, `endDate` | ViewClassroom |
| GET | `/{classroomId}/schedule` | Get classroom schedule | `classroomId` (GUID) | ViewClassroom |
| POST | `/` | Create new classroom | CreateClassroomDTO in body | ManageClassroom |
| PUT | `/` | Update existing classroom | UpdateClassroomDTO in body | ManageClassroom |
| DELETE | `/{classroomId}` | Delete classroom | `classroomId` (GUID) | DeleteClassroom |

### Query Parameters
- **status**: Filter by classroom status (Available, Occupied, Maintenance, Reserved)
- **minCapacity**: Filter by minimum capacity requirement
- **name**: Filter by room name (partial match supported)
- **startDate**: Start date for availability check (YYYY-MM-DD format)
- **endDate**: End date for availability check (YYYY-MM-DD format)

## Data Transfer Objects (DTOs)

### ClassroomBaseDTO (Abstract)
```csharp
public abstract record class ClassroomBaseDTO
{
    [Required, StringLength(20)]
    public string RoomCode { get; set; } = string.Empty;
    
    [Required, StringLength(100)]
    public string RoomName { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? Building { get; set; }
    
    [Range(0, int.MaxValue)]
    public int? FloorLevel { get; set; }
    
    [Required, Range(1, int.MaxValue)]
    public int Capacity { get; set; }
    
    public string? Equipment { get; set; }
    public string? Facilities { get; set; }
    public string? LocationNotes { get; set; }
    
    [StringLength(20)]
    public string Status { get; set; } = "Available";
}
```

### CreateClassroomDTO
- Inherits from ClassroomBaseDTO
- Additional: `CreatedBy` (auto-populated from JWT claims)

### UpdateClassroomDTO
- Inherits from ClassroomBaseDTO
- Additional: `ClassroomId`, `ModifiedBy` (auto-populated from JWT claims)

### GetClassroomDTO
- Inherits from ClassroomBaseDTO
- Additional: `ClassroomId`, `CreatedBy`, `ModifiedBy`, `CreateDate`, `UpdateDate`

## Application Layer Usage

### Controller Actions

#### 1. Index (Main View)
- **Route**: `GET /Classroom`
- **Purpose**: Display paginated list of all classrooms
- **Features**: Search functionality, pagination, classroom creation
- **Permissions**: ViewClassroom

#### 2. ListClassroom (AJAX Partial View)
- **Route**: `GET /Classroom/ListClassroom`
- **Purpose**: Refresh classroom list with search filters
- **Parameters**: `ListClassroomDTOs` (includes SearchText, Page, PageSize)
- **Returns**: Partial view with filtered results

#### 3. FetchClassroom (API for JavaScript)
- **Route**: `GET /Classroom/FetchClassroom`
- **Purpose**: Get classroom data for JavaScript/AJAX calls
- **Parameters**: `i` (classroom name filter)
- **Returns**: JSON array of classroom objects

#### 4. AddClassroom
- **GET Route**: `GET /Classroom/AddClassroom` - Show create form
- **POST Route**: `POST /Classroom/AddClassroom` - Process creation
- **Model**: `CreateClassroomDTO`
- **Features**: Equipment tracking, facility management
- **Permissions**: ManageClassroom

#### 5. EditClassroom
- **GET Route**: `GET /Classroom/EditClassroom/{id}` - Show edit form
- **POST Route**: `POST /Classroom/EditClassroom` - Process update
- **Model**: `UpdateClassroomDTO`
- **Features**: Pre-populated form, validation
- **Permissions**: ManageClassroom

#### 6. DeleteClassroom
- **GET Route**: `GET /Classroom/DeleteClassroom/{id}` - Show confirmation
- **POST Route**: `POST /Classroom/DeleteClassroomConfirmed` - Process deletion
- **Permissions**: DeleteClassroom

#### 7. GetAvailableClassrooms
- **Route**: `GET /Classroom/GetAvailableClassrooms`
- **Purpose**: Get available classrooms for scheduling
- **Returns**: JSON array for AJAX consumption
- **Used by**: Class scheduling, room booking

#### 8. GetClassroomSchedule
- **Route**: `GET /Classroom/GetClassroomSchedule/{id}`
- **Purpose**: Get classroom schedule and bookings
- **Returns**: JSON object with schedule data

## Frontend Implementation

### Views Structure
```
Views/Classroom/
├── Index.cshtml                 # Main classrooms listing page
├── _ListClassrooms.cshtml      # Partial view for classrooms table
├── _addClassroom.cshtml        # Modal form for creating classrooms
├── _editClassroom.cshtml       # Modal form for editing classrooms
└── _deleteClassroom.cshtml     # Modal confirmation for deletion
```

### Key Features

#### 1. Search and Filtering
- **Current Implementation**: Search by room name via SearchText
- **API Capability**: Supports status, minCapacity, name filters
- **Enhancement Opportunity**: Could implement advanced filtering UI

#### 2. Pagination
- Uses `PageList<GetClassroomDTO>` for pagination
- Configurable page size (default: 10, options: 20, 50, 100)
- Maintains search filters across pages

#### 3. Status Display
- **Available**: Green badge for available classrooms
- **Occupied**: Yellow badge for occupied classrooms
- **Maintenance**: Red badge for maintenance
- **Reserved**: Blue badge for reserved classrooms

#### 4. Actions Menu
- **Edit**: Available for all classrooms
- **Schedule**: View classroom schedule and bookings
- **Delete**: Available with proper permissions

#### 5. Form Features
- **Room Information**: Code, name, building, floor level
- **Capacity Management**: Seating capacity with validation
- **Equipment Tracking**: Detailed equipment list
- **Facilities Management**: Available facilities and amenities
- **Status Management**: Current status with dropdown selection
- **Location Notes**: Additional location information
- **Validation**: Client and server-side validation

## Service Layer

### ClassroomService Implementation
```csharp
public class ClassroomService : BaseApiService, IClassroomService
{
    // Implements all CRUD operations
    // Handles HTTP communication with API
    // Manages query parameter construction
    // Provides typed responses
}
```

### Key Methods
- `GetAllClassrooms()`: Retrieve classrooms with optional filters
- `GetClassroom()`: Get specific classroom by ID
- `GetAvailableClassrooms()`: Get only available classrooms
- `GetClassroomSchedule()`: Get classroom schedule and bookings
- `CreateClassroom()`: Create new classroom
- `UpdateClassroom()`: Update existing classroom
- `DeleteClassroom()`: Delete classroom by ID

## Business Logic

### 1. Classroom Creation Workflow
1. User clicks "Add Classroom" button
2. Modal opens with classroom creation form
3. User fills required information:
   - Room Code (unique identifier)
   - Room Name (descriptive name)
   - Building and floor level
   - Seating capacity
   - Equipment and facilities
   - Status (default: Available)
4. System validates business rules
5. Classroom created with specified properties
6. Available for scheduling and booking

### 2. Status Management
- **Available**: Ready for use and scheduling
- **Occupied**: Currently in use
- **Maintenance**: Under maintenance, not available
- **Reserved**: Reserved for specific purpose/event

### 3. Capacity Planning
- Each classroom has defined seating capacity
- Used for class size planning and student allocation
- Prevents overbooking and ensures adequate space

### 4. Equipment and Facilities
- **Equipment**: Projectors, computers, whiteboards, etc.
- **Facilities**: WiFi, power outlets, storage, air conditioning
- **Location Notes**: Special instructions or accessibility information

## Validation Rules

### Business Validation
- Room codes must be unique
- Capacity must be positive integer
- Floor level must be non-negative
- Status must be valid (Available, Occupied, Maintenance, Reserved)

### Form Validation
- Required fields enforced
- String length limits
- Numeric range validations
- Client-side real-time validation
- Server-side validation as backup

## Integration Points

- **Class Scheduling**: Classrooms linked to scheduled classes
- **Student Management**: Capacity planning for student enrollment
- **Resource Management**: Equipment and facility tracking
- **Maintenance**: Maintenance scheduling and status updates
- **Reporting**: Classroom utilization and occupancy reports
- **Booking System**: Integration with room booking functionality

## Performance Considerations

- **Pagination**: Large classroom catalogs handled efficiently
- **Caching**: Consider caching available classrooms for frequent access
- **Indexing**: Database indexes on frequently queried fields
- **Lazy Loading**: Partial views for better page load times
- **Search Optimization**: Efficient filtering and search

## Security Considerations

- **Authentication**: All endpoints require valid JWT token
- **Authorization**: Role-based permissions enforced
- **Input Validation**: Server-side validation on all DTOs
- **CSRF Protection**: Anti-forgery tokens on all forms
- **Audit Trail**: CreatedBy/ModifiedBy tracking on all changes
- **Data Protection**: Soft delete for classroom history preservation

## Reporting and Analytics

- Classroom utilization statistics
- Equipment usage and maintenance tracking
- Capacity utilization analysis
- Building and floor occupancy reports
- Maintenance scheduling and history
- Booking patterns and trends

## Future Enhancements

1. **Advanced Search**: Multi-criteria search with all filter options
2. **Room Booking**: Direct booking functionality from classroom list
3. **Bulk Operations**: Mass update classroom information
4. **Classroom Templates**: Predefined classroom templates for quick creation
5. **Integration APIs**: External system integration capabilities
6. **Mobile Optimization**: Responsive design improvements
7. **Real-time Status**: Live status updates and notifications
8. **Maintenance Scheduling**: Integrated maintenance management
9. **Capacity Analytics**: Advanced capacity utilization reporting
10. **Equipment Tracking**: Detailed equipment lifecycle management

## Usage Examples

### Creating a New Classroom
1. Navigate to Classrooms management
2. Click "Add Classroom" button
3. Fill in classroom details:
   - Code: "A101"
   - Name: "Main Classroom A101"
   - Building: "Main Building"
   - Floor: 1
   - Capacity: 30
   - Equipment: "Projector, Whiteboard, Computer"
4. Set status and facilities
5. Save classroom

### Using Classrooms in Scheduling
1. Open Class scheduling form
2. Available classrooms load via AJAX
3. Select appropriate classroom for the class
4. System checks availability and capacity
5. Classroom is assigned to the class

### Checking Classroom Availability
1. Admin checks classroom availability
2. System filters by date range and status
3. Available classrooms displayed
4. Capacity requirements considered
5. Booking conflicts prevented

## API Response Examples

### GetAvailableClassrooms Response
```json
[
  {
    "id": "guid-here",
    "text": "A101 - Main Classroom A101",
    "code": "A101",
    "name": "Main Classroom A101",
    "building": "Main Building",
    "capacity": 30,
    "status": "Available"
  }
]
```

### GetClassroom Response
```json
{
  "classroomId": "guid-here",
  "roomCode": "A101",
  "roomName": "Main Classroom A101",
  "building": "Main Building",
  "floorLevel": 1,
  "capacity": 30,
  "equipment": "Projector, Whiteboard, Computer",
  "facilities": "WiFi, Power outlets, Air conditioning",
  "status": "Available",
  "locationNotes": "Near main entrance, accessible"
}
```

### GetClassroomSchedule Response
```json
[
  {
    "classId": "guid-here",
    "className": "Chinese Level 1",
    "startTime": "09:00",
    "endTime": "10:30",
    "date": "2024-01-15",
    "instructor": "John Doe",
    "studentCount": 25
  }
]
```

This comprehensive Classroom management system provides a robust foundation for physical resource administration within the Hong Wen education platform.
