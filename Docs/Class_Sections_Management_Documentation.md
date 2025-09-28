# Class Sections Management Documentation

## Overview

The Class Sections Management system provides comprehensive functionality for managing class sections within the Hong Wen School Management System. This module allows administrators to create, manage, and track class sections, including enrollment, scheduling, and status management.

## Table of Contents

1. [Business Workflow](#business-workflow)
2. [System Architecture](#system-architecture)
3. [Features and Functionality](#features-and-functionality)
4. [User Interface Guide](#user-interface-guide)
5. [API Integration](#api-integration)
6. [Data Models](#data-models)
7. [Permissions and Security](#permissions-and-security)
8. [Usage Examples](#usage-examples)
9. [Troubleshooting](#troubleshooting)

## Business Workflow

### 1. Section Creation Workflow

```
1. Administrator accesses Class Sections module
2. Clicks "Add Section" button
3. Fills in section details:
   - Section Code (unique identifier)
   - Section Name (optional)
   - Course selection
   - Term selection
   - Teacher assignment
   - Classroom assignment
   - Schedule pattern
   - Enrollment limits
   - Fee structure
4. System validates:
   - Section code uniqueness
   - Date conflicts
   - Teacher availability
   - Classroom availability
5. Section created with "Planning" status
6. Section appears in the list
```

### 2. Section Status Lifecycle

```
Planning → Open → Running → Completed
    ↓         ↓        ↓
Cancelled  Cancelled  Cancelled
```

**Status Transitions:**
- **Planning**: Initial state, section being prepared
- **Open**: Accepting enrollments
- **Full**: Maximum enrollment reached
- **Running**: Section is active and in progress
- **Completed**: Section has finished
- **Cancelled**: Section cancelled at any stage

### 3. Section Management Workflow

```
1. View sections with filtering options
2. Edit section details (if status allows)
3. Update section status as needed
4. Duplicate sections for new terms
5. Delete sections (with restrictions)
6. Monitor enrollment and waitlist
```

## System Architecture

### Components

1. **Frontend (APP)**
   - `ClassSectionController` - Handles HTTP requests
   - `ClassSectionService` - Business logic layer
   - Views - User interface components
   - DTOs - Data transfer objects

2. **Backend (API)**
   - `ClassSectionController` - REST API endpoints
   - `ClassSectionRepository` - Data access layer
   - `IClassSection` - Service interface
   - Entity models

### Data Flow

```
User Interface → Controller → Service → API → Repository → Database
                ↓
            Response ← Service ← API ← Repository ← Database
```

## Features and Functionality

### 1. Section Management

#### Create Section
- **Purpose**: Create new class sections
- **Access**: Users with "ManageClassSections" permission
- **Features**:
  - Form validation
  - Dropdown selections for related entities
  - Date range validation
  - Fee structure setup
  - Schedule pattern input

#### Edit Section
- **Purpose**: Modify existing section details
- **Access**: Users with "ManageClassSections" permission
- **Features**:
  - Pre-populated form with current data
  - Validation against current status
  - Audit trail display
  - Status-aware editing restrictions

#### Delete Section
- **Purpose**: Remove sections from the system
- **Access**: Users with "DeleteClassSections" permission
- **Restrictions**:
  - Cannot delete active or completed sections
  - Cannot delete sections with existing enrollments
  - Confirmation dialog required

### 2. Status Management

#### Status Updates
- **Purpose**: Change section status throughout lifecycle
- **Access**: Users with "ManageClassSections" permission
- **Features**:
  - Quick status change buttons
  - Status transition validation
  - Automatic status updates based on enrollment

#### Status Actions
- **Planning → Open**: Make section available for enrollment
- **Open → Running**: Start the section
- **Running → Completed**: Mark section as finished
- **Any → Cancelled**: Cancel the section

### 3. Section Duplication

#### Duplicate Section
- **Purpose**: Create new sections based on existing ones
- **Access**: Users with "ManageClassSections" permission
- **Features**:
  - Copy all section details
  - Change section code and term
  - Reset enrollment counts
  - Set status to "Planning"

### 4. Filtering and Search

#### Advanced Filtering
- **Status Filter**: Filter by section status
- **Course Filter**: Filter by course
- **Term Filter**: Filter by term
- **Teacher Filter**: Filter by assigned teacher
- **Search**: Text search across section codes and names

## User Interface Guide

### 1. Main Section List

#### Layout
- **Header**: Title, search bar, add button
- **Filters**: Status, course, term, teacher dropdowns
- **Table**: Section details with action buttons
- **Pagination**: Page navigation controls

#### Table Columns
- **No**: Row number
- **Section Code**: Unique identifier
- **Section Name**: Descriptive name
- **Course**: Associated course
- **Teacher**: Assigned teacher
- **Classroom**: Assigned classroom
- **Term**: Associated term
- **Enrollment**: Current/Max enrollment with waitlist
- **Status**: Current status with color coding
- **Actions**: Edit, duplicate, status changes, delete

#### Status Color Coding
- **Planning**: Gray
- **Open**: Green
- **Full**: Yellow
- **Running**: Blue
- **Completed**: Light blue
- **Cancelled**: Red

### 2. Add Section Form

#### Form Sections
1. **Basic Information**
   - Section Code (required)
   - Section Name (optional)

2. **Assignments**
   - Course selection (required)
   - Term selection (required)
   - Teacher selection (required)
   - Classroom selection (required)

3. **Schedule**
   - Start Date (required)
   - End Date (required)
   - Schedule Pattern (required)

4. **Enrollment**
   - Maximum Enrollment (required)
   - Current Enrollment (optional)
   - Waitlist Count (optional)

5. **Fees**
   - Tuition Fee (required)
   - Materials Fee (optional)
   - Registration Fee (optional)

6. **Additional**
   - Status selection
   - Notes (optional)

### 3. Edit Section Form

#### Features
- Pre-populated with current data
- Read-only audit information
- Status-aware field restrictions
- Validation against current state

### 4. Delete Confirmation

#### Information Display
- Section details for confirmation
- Warning messages
- Deletion restrictions
- Confirmation buttons

### 5. Duplicate Section Form

#### Source Information
- Display of original section details
- Read-only source data

#### New Section Details
- New section code input
- New term selection
- Information about what will be copied

## API Integration

### Endpoints

#### GET /sections
- **Purpose**: Retrieve all sections with optional filtering
- **Parameters**: sectionCode, sectionName, status, courseId, termId, teacherId
- **Response**: List of GetClassSectionDTO

#### GET /sections/{sectionId}
- **Purpose**: Retrieve specific section
- **Response**: GetClassSectionDTO

#### POST /sections
- **Purpose**: Create new section
- **Body**: CreateClassSectionDTO
- **Response**: Response object

#### PUT /sections
- **Purpose**: Update existing section
- **Body**: UpdateClassSectionDTO
- **Response**: Response object

#### DELETE /sections/{sectionId}
- **Purpose**: Delete section
- **Response**: Response object

#### POST /sections/{sectionId}/duplicate
- **Purpose**: Duplicate section
- **Body**: DuplicateClassSectionDTO
- **Response**: Response object

#### PUT /sections/{sectionId}/status
- **Purpose**: Update section status
- **Body**: UpdateSectionStatusDTO
- **Response**: Response object

### Service Layer

#### ClassSectionService
- **Purpose**: Business logic and API communication
- **Methods**:
  - `GetClassSection(Guid sectionId)`
  - `GetAllClassSections(...)`
  - `CreateClassSection(CreateClassSectionDTO)`
  - `UpdateClassSection(UpdateClassSectionDTO)`
  - `DeleteClassSection(Guid sectionId)`
  - `DuplicateClassSection(Guid sectionId, DuplicateClassSectionDTO)`
  - `UpdateSectionStatus(Guid sectionId, string status)`
  - `ValidateScheduleConflicts(...)`

## Data Models

### DTOs

#### ClassSectionBaseDTO
- **Purpose**: Base class for section data
- **Properties**:
  - CourseId, TermId, TeacherId, ClassroomId
  - SectionCode, SectionName
  - StartDate, EndDate, SchedulePattern
  - MaxEnrollment, CurrentEnrollment, WaitlistCount
  - TuitionFee, MaterialsFee, RegistrationFee
  - Status, Notes

#### CreateClassSectionDTO
- **Purpose**: Data for creating new sections
- **Inherits**: ClassSectionBaseDTO
- **Additional**: CreatedBy

#### UpdateClassSectionDTO
- **Purpose**: Data for updating sections
- **Inherits**: ClassSectionBaseDTO
- **Additional**: SectionId, ModifiedBy

#### GetClassSectionDTO
- **Purpose**: Data for displaying sections
- **Inherits**: ClassSectionBaseDTO
- **Additional**: 
  - SectionId, CreatedBy, ModifiedBy
  - CreateDate, UpdateDate
  - CourseName, CourseCode, TermName
  - TeacherName, ClassroomName

#### DuplicateClassSectionDTO
- **Purpose**: Data for duplicating sections
- **Properties**: NewSectionCode, NewTermId

#### UpdateSectionStatusDTO
- **Purpose**: Data for status updates
- **Properties**: Status, ModifiedBy

#### ListClassSectionDTOs
- **Purpose**: Data for list view with filtering
- **Properties**: 
  - classSection (PageList)
  - Page, PageSize, SearchText
  - StatusFilter, CourseFilter, TermFilter, TeacherFilter

## Permissions and Security

### Required Permissions

#### ViewClassSections
- **Purpose**: View section list and details
- **Required for**: All read operations
- **Users**: Administrators, Academic staff

#### ManageClassSections
- **Purpose**: Create, edit, and update sections
- **Required for**: CRUD operations, status changes
- **Users**: Administrators, Academic coordinators

#### DeleteClassSections
- **Purpose**: Delete sections
- **Required for**: Section deletion
- **Users**: Administrators only

### Security Features

#### Data Validation
- Server-side validation for all inputs
- Date range validation
- Unique constraint enforcement
- Status transition validation

#### Access Control
- Permission-based access to features
- Role-based UI element visibility
- Audit trail for all changes

#### Data Protection
- Input sanitization
- SQL injection prevention
- XSS protection
- CSRF protection

## Usage Examples

### Example 1: Creating a New Section

```csharp
// 1. User clicks "Add Section"
// 2. Form loads with dropdowns populated
// 3. User fills in details:
var newSection = new CreateClassSectionDTO
{
    SectionCode = "CHN101-A",
    SectionName = "Chinese Beginner A",
    CourseId = courseId,
    TermId = termId,
    TeacherId = teacherId,
    ClassroomId = classroomId,
    StartDate = DateTime.Parse("2024-01-15"),
    EndDate = DateTime.Parse("2024-04-15"),
    SchedulePattern = "Monday 9:00-11:00, Wednesday 9:00-11:00",
    MaxEnrollment = 25,
    TuitionFee = 150.00m,
    Status = "Planning"
};

// 4. System validates and creates section
// 5. Success message displayed
// 6. Section appears in list
```

### Example 2: Updating Section Status

```csharp
// 1. User clicks status action button
// 2. Confirmation dialog appears
// 3. User confirms status change
// 4. System updates status
await classSectionService.UpdateSectionStatus(sectionId, "Open");

// 5. Status updated in database
// 6. UI refreshed with new status
// 7. Success message displayed
```

### Example 3: Filtering Sections

```csharp
// 1. User selects filters
// 2. Form submits with filter parameters
var filteredSections = await classSectionService.GetAllClassSections(
    status: "Open",
    courseId: selectedCourseId,
    termId: selectedTermId
);

// 3. Filtered results displayed
// 4. Pagination updated
// 5. Filter state maintained
```

### Example 4: Duplicating a Section

```csharp
// 1. User clicks "Duplicate" on existing section
// 2. Duplicate form loads with source information
// 3. User enters new details
var duplicateDto = new DuplicateClassSectionDTO
{
    NewSectionCode = "CHN101-A_COPY",
    NewTermId = newTermId
};

// 4. System creates new section with copied data
// 5. New section appears in list with "Planning" status
// 6. Success message displayed
```

## Troubleshooting

### Common Issues

#### 1. Section Code Already Exists
- **Problem**: Error when creating/editing section
- **Solution**: Choose a unique section code
- **Prevention**: System validates uniqueness

#### 2. Cannot Delete Section
- **Problem**: Delete button disabled or error message
- **Causes**: 
  - Section is running or completed
  - Section has existing enrollments
- **Solution**: 
  - Cancel section first
  - Remove enrollments
  - Or wait for section completion

#### 3. Status Change Not Allowed
- **Problem**: Status change button not working
- **Causes**: Invalid status transition
- **Solution**: Follow proper status lifecycle
- **Valid Transitions**:
  - Planning → Open, Cancelled
  - Open → Full, Running, Cancelled
  - Full → Open, Running, Cancelled
  - Running → Completed, Cancelled

#### 4. Schedule Conflicts
- **Problem**: Validation error for teacher/classroom
- **Causes**: Overlapping schedules
- **Solution**: 
  - Check teacher availability
  - Check classroom availability
  - Adjust schedule or assignments

#### 5. Date Validation Errors
- **Problem**: Start/end date errors
- **Causes**: 
  - End date before start date
  - Dates outside term range
- **Solution**: 
  - Ensure end date after start date
  - Check term date ranges

### Error Messages

#### Validation Errors
- "Section code already exists"
- "End date must be after start date"
- "Section dates must be within term dates"
- "Schedule conflicts detected"
- "Invalid status transition"

#### Permission Errors
- "Access denied"
- "Insufficient permissions"
- "Unauthorized action"

#### System Errors
- "Error retrieving Class Section"
- "Error updating Class Section"
- "Error deleting Class Section"
- "Error duplicating Class Section"

### Debugging Tips

#### 1. Check Permissions
- Verify user has required permissions
- Check role assignments
- Review permission configuration

#### 2. Validate Data
- Check all required fields
- Verify date formats
- Ensure unique identifiers

#### 3. Review Status
- Check current section status
- Verify status transition rules
- Review business logic

#### 4. Check Dependencies
- Verify course exists
- Check teacher availability
- Confirm classroom availability
- Validate term dates

#### 5. Monitor Logs
- Check application logs
- Review API logs
- Monitor database logs
- Check error messages

### Performance Considerations

#### 1. Large Datasets
- Use pagination for large section lists
- Implement efficient filtering
- Consider caching for dropdown data

#### 2. Concurrent Updates
- Handle concurrent status updates
- Implement optimistic locking
- Use transaction management

#### 3. Validation Performance
- Cache validation results
- Optimize database queries
- Use efficient conflict detection

## Conclusion

The Class Sections Management system provides a comprehensive solution for managing class sections within the Hong Wen School Management System. It offers intuitive user interfaces, robust business logic, and secure data management capabilities.

Key benefits:
- **Efficiency**: Streamlined section management workflow
- **Flexibility**: Multiple status options and transition paths
- **Security**: Permission-based access control
- **Usability**: Clean, intuitive user interface
- **Scalability**: Designed for growth and expansion
- **Integration**: Seamless integration with other system modules

The system follows established patterns and best practices, ensuring maintainability and extensibility for future enhancements.
