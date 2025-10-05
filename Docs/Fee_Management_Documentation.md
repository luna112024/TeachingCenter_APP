# Fee Management System Documentation

## Overview
The Fee Management System handles comprehensive fee administration within the Hong Wen education platform. It provides functionality for managing fee categories, fee templates, student fee assignments, payment tracking, and financial reporting across three main components: Fee Categories, Fee Templates, and Student Fees.

## Business Workflow

### 1. Fee Management Lifecycle
```
Fee Category Creation → Fee Template Setup → Student Fee Assignment → Payment Processing → Financial Reporting
```

### 2. Key Business Rules
- **Fee Categories**: Organize fees by type (Tuition, Registration, Materials, etc.)
- **Fee Templates**: Define standard fee structures with amounts and rules
- **Student Fees**: Individual fee assignments with payment tracking
- **Unique Codes**: All fee components must have unique identifiers
- **Status Tracking**: Active, Inactive, Pending, Paid, Overdue, Waived status management
- **Payment Integration**: Integration with payment processing systems
- **Late Fee Management**: Automatic late fee calculation and application
- **Discount Management**: Support for various discount types and reasons

### 3. User Roles and Permissions
- **ViewFee**: Can view fee information and financial records
- **ManageFee**: Can create and edit fees, assign fees to students
- **DeleteFee**: Can delete fees (separate permission for data protection)

## API Endpoints

### Base URL: `/sms/Fee`

#### Fee Categories
| Method | Endpoint | Description | Parameters | Permission |
|--------|----------|-------------|------------|------------|
| GET | `/categories` | Get all fee categories | None | ViewFee |
| GET | `/categories/{categoryId}` | Get specific category | `categoryId` (GUID) | ViewFee |
| POST | `/categories` | Create new category | CreateFeeCategoryDTO in body | ManageFee |
| PUT | `/categories` | Update existing category | UpdateFeeCategoryDTO in body | ManageFee |
| DELETE | `/categories/{categoryId}` | Delete category | `categoryId` (GUID) | DeleteFee |

#### Fee Templates
| Method | Endpoint | Description | Parameters | Permission |
|--------|----------|-------------|------------|------------|
| GET | `/templates` | Get all fee templates | None | ViewFee |
| GET | `/templates/{templateId}` | Get specific template | `templateId` (GUID) | ViewFee |
| GET | `/templates/category/{categoryId}` | Get templates by category | `categoryId` (GUID) | ViewFee |
| GET | `/templates/type/{feeType}` | Get templates by type | `feeType` (string) | ViewFee |
| POST | `/templates` | Create new template | CreateFeeTemplateDTO in body | ManageFee |
| PUT | `/templates` | Update existing template | UpdateFeeTemplateDTO in body | ManageFee |
| DELETE | `/templates/{templateId}` | Delete template | `templateId` (GUID) | DeleteFee |

#### Student Fees
| Method | Endpoint | Description | Parameters | Permission |
|--------|----------|-------------|------------|------------|
| GET | `/student-fees/{studentId}` | Get student fees | `studentId` (GUID) | ViewFee |
| GET | `/student-fees/{studentFeeId}/detail` | Get specific student fee | `studentFeeId` (GUID) | ViewFee |
| GET | `/student-fees/enrollment/{enrollmentId}` | Get fees by enrollment | `enrollmentId` (GUID) | ViewFee |
| POST | `/student-fees/assign` | Assign fee to student | AssignFeeToStudentDTO in body | ManageFee |
| PUT | `/student-fees` | Update student fee | UpdateStudentFeeDTO in body | ManageFee |
| PUT | `/student-fees/{studentFeeId}/waive` | Waive student fee | `studentFeeId` (GUID), WaiveStudentFeeDTO in body | ManageFee |
| DELETE | `/student-fees/{studentFeeId}` | Delete student fee | `studentFeeId` (GUID) | DeleteFee |

#### Business Operations
| Method | Endpoint | Description | Parameters | Permission |
|--------|----------|-------------|------------|------------|
| GET | `/overdue` | Get overdue fees | `fromDate`, `toDate` (optional) | ViewFee |
| GET | `/pending` | Get pending fees | `studentId` (optional) | ViewFee |
| POST | `/calculate-late-fees` | Calculate late fees | None | ManageFee |
| POST | `/generate-invoices` | Generate invoices | `studentId`, `dueDate` (optional) | ManageFee |

## Data Transfer Objects (DTOs)

### Fee Category DTOs

#### FeeCategoryBaseDTO (Abstract)
```csharp
public abstract record class FeeCategoryBaseDTO
{
    [Required, StringLength(100)]
    public string CategoryName { get; set; }
    
    [Required, StringLength(20)]
    public string CategoryCode { get; set; }
    
    public string? Description { get; set; }
    
    public bool IsMandatory { get; set; } = true;
    
    public int DisplayOrder { get; set; } = 0;
    
    [StringLength(20)]
    public string Status { get; set; } = "Active";
}
```

#### CreateFeeCategoryDTO
- Inherits from FeeCategoryBaseDTO
- Additional: `CreatedBy` (auto-populated from JWT claims)

#### UpdateFeeCategoryDTO
- Inherits from FeeCategoryBaseDTO
- Additional: `CategoryId`, `ModifiedBy` (auto-populated from JWT claims)

#### GetFeeCategoryDTO
- Inherits from FeeCategoryBaseDTO
- Additional: `CategoryId`, `CreatedBy`, `CreateDate`

### Fee Template DTOs

#### FeeTemplateBaseDTO (Abstract)
```csharp
public abstract record class FeeTemplateBaseDTO
{
    [Required, StringLength(200)]
    public string TemplateName { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }
    
    [Required, StringLength(20)]
    public string FeeType { get; set; } // 'Tuition', 'Registration', 'Materials', etc.
    
    [Required, Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal BaseAmount { get; set; }
    
    [StringLength(3)]
    public string Currency { get; set; } = "USD";
    
    [StringLength(50)]
    public string ApplicableTo { get; set; } = "All";
    
    public string? ApplicableLevels { get; set; } // JSON string for levels array
    
    public int DueDaysAfterEnrollment { get; set; } = 7;
    
    public int LateFeeDays { get; set; } = 7;
    
    public decimal LateFeeAmount { get; set; } = 0.00m;
    
    public decimal EarlyPaymentDiscountPercent { get; set; } = 0.00m;
    
    public int EarlyPaymentDays { get; set; } = 0;
    
    public decimal SiblingDiscountPercent { get; set; } = 0.00m;
    
    [Required]
    public DateTime EffectiveDate { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    [StringLength(20)]
    public string Status { get; set; } = "Active";
}
```

#### CreateFeeTemplateDTO
- Inherits from FeeTemplateBaseDTO
- Additional: `CreatedBy` (auto-populated from JWT claims)

#### UpdateFeeTemplateDTO
- Inherits from FeeTemplateBaseDTO
- Additional: `TemplateId`, `ModifiedBy` (auto-populated from JWT claims)

#### GetFeeTemplateDTO
- Inherits from FeeTemplateBaseDTO
- Additional: `TemplateId`, `CreatedBy`, `CreateDate`, `CategoryName`

### Student Fee DTOs

#### StudentFeeBaseDTO (Abstract)
```csharp
public abstract record class StudentFeeBaseDTO
{
    [Required]
    public Guid StudentId { get; set; }
    
    public Guid? EnrollmentId { get; set; }
    
    public Guid? TemplateId { get; set; }
    
    [Required, StringLength(200)]
    public string FeeName { get; set; }
    
    [Required, StringLength(20)]
    public string FeeType { get; set; }
    
    [Required, Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal OriginalAmount { get; set; }
    
    public decimal DiscountAmount { get; set; } = 0.00m;
    
    public string? DiscountReason { get; set; }
    
    [Required]
    public decimal FinalAmount { get; set; }
    
    [StringLength(3)]
    public string Currency { get; set; } = "USD";
    
    [Required]
    public DateTime DueDate { get; set; }
    
    public int GracePeriodDays { get; set; } = 0;
    
    public decimal LateFeeApplied { get; set; } = 0.00m;
    
    [StringLength(20)]
    public string Status { get; set; } = "Pending";
    
    public decimal AmountPaid { get; set; } = 0.00m;
    
    public decimal AmountOutstanding { get; set; }
    
    [StringLength(50)]
    public string? InvoiceNumber { get; set; }
    
    public string? Notes { get; set; }
}
```

#### CreateStudentFeeDTO
- Inherits from StudentFeeBaseDTO
- Additional: `CreatedBy` (auto-populated from JWT claims)

#### UpdateStudentFeeDTO
- Inherits from StudentFeeBaseDTO
- Additional: `StudentFeeId`, `ModifiedBy` (auto-populated from JWT claims)

#### GetStudentFeeDTO
- Inherits from StudentFeeBaseDTO
- Additional: `StudentFeeId`, `CreatedBy`, `ModifiedBy`, `CreateDate`, `UpdateDate`
- Display fields: `StudentName`, `StudentCode`, `CourseName`, `SectionCode`, `TemplateName`, `CategoryName`

#### AssignFeeToStudentDTO
```csharp
public record class AssignFeeToStudentDTO
{
    [Required]
    public Guid StudentId { get; set; }
    
    [Required]
    public Guid TemplateId { get; set; }
    
    public Guid? EnrollmentId { get; set; }
    
    public decimal? CustomAmount { get; set; }
    
    public string? CustomDueDate { get; set; }
    
    public string? Notes { get; set; }
    
    // Additional fields for fee calculation
    public decimal OriginalAmount { get; set; }
    public decimal DiscountAmount { get; set; } = 0.00m;
    public string? DiscountReason { get; set; }
    public decimal FinalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime DueDate { get; set; }
    public int GracePeriodDays { get; set; } = 0;
    public decimal LateFeeApplied { get; set; } = 0.00m;
    public string Status { get; set; } = "Pending";
    public decimal AmountPaid { get; set; } = 0.00m;
    public decimal AmountOutstanding { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
}
```

#### WaiveStudentFeeDTO
```csharp
public record class WaiveStudentFeeDTO
{
    [Required]
    public Guid StudentFeeId { get; set; }
    
    [Required]
    public string WaiveReason { get; set; }
    
    [Required]
    public string WaivedReason { get; set; }
    
    public string? WaivedBy { get; set; }
    public string? ModifiedBy { get; set; }
}
```

## Application Layer Usage

### Controllers

#### 1. FeeCategoryController
- **Purpose**: Manage fee categories
- **Actions**: Index, ListFeeCategory, FetchFeeCategory, AddFeeCategory, EditFeeCategory, DeleteFeeCategory, DetailsFeeCategory
- **Features**: Category management, mandatory flag, display order

#### 2. FeeTemplateController
- **Purpose**: Manage fee templates
- **Actions**: Index, ListFeeTemplate, FetchFeeTemplate, AddFeeTemplate, EditFeeTemplate, DeleteFeeTemplate, DetailsFeeTemplate
- **Features**: Template management, fee type configuration, discount settings

#### 3. StudentFeeController
- **Purpose**: Manage student fee assignments
- **Actions**: Index, ListStudentFee, FetchStudentFee, AssignFeeToStudent, EditStudentFee, DeleteStudentFee, DetailsStudentFee, WaiveStudentFee
- **Features**: Fee assignment, payment tracking, waiver management, business operations

## Frontend Implementation

### Views Structure
```
Views/FeeCategory/
├── Index.cshtml                 # Main fee categories listing page
├── _ListFeeCategories.cshtml    # Partial view for categories table
├── _addFeeCategory.cshtml       # Modal form for creating categories
├── _editFeeCategory.cshtml      # Modal form for editing categories
├── _deleteFeeCategory.cshtml    # Modal confirmation for deletion
└── _detailsFeeCategory.cshtml   # Modal for category details

Views/FeeTemplate/
├── Index.cshtml                 # Main fee templates listing page
├── _ListFeeTemplates.cshtml     # Partial view for templates table
├── _addFeeTemplate.cshtml       # Modal form for creating templates
├── _editFeeTemplate.cshtml      # Modal form for editing templates
├── _deleteFeeTemplate.cshtml    # Modal confirmation for deletion
└── _detailsFeeTemplate.cshtml   # Modal for template details

Views/StudentFee/
├── Index.cshtml                 # Main student fees listing page
├── _ListStudentFees.cshtml      # Partial view for student fees table
├── _assignFeeToStudent.cshtml   # Modal form for assigning fees
├── _editStudentFee.cshtml       # Modal form for editing student fees
├── _deleteStudentFee.cshtml     # Modal confirmation for deletion
├── _detailsStudentFee.cshtml    # Modal for student fee details
└── _waiveStudentFee.cshtml      # Modal form for waiving fees
```

### Key Features

#### 1. Fee Categories Management
- **Category Creation**: Create fee categories with mandatory flags
- **Display Order**: Organize categories by display order
- **Status Management**: Active/Inactive status tracking
- **Validation**: Unique category names and codes

#### 2. Fee Templates Management
- **Template Creation**: Create fee templates with base amounts
- **Fee Types**: Support for various fee types (Tuition, Registration, Materials, etc.)
- **Currency Support**: Multiple currency support (USD, KHR, EUR)
- **Discount Configuration**: Early payment and sibling discounts
- **Late Fee Settings**: Late fee calculation and application
- **Applicability Rules**: Define which students/levels templates apply to

#### 3. Student Fee Management
- **Fee Assignment**: Assign fees to students using templates
- **Payment Tracking**: Track payments and outstanding amounts
- **Status Management**: Pending, Paid, Partial, Overdue, Waived, Cancelled
- **Discount Application**: Apply discounts with reasons
- **Fee Waiver**: Waive fees with proper documentation
- **Invoice Generation**: Generate invoices for fees

#### 4. Business Operations
- **Overdue Fee Management**: Track and manage overdue fees
- **Late Fee Calculation**: Automatic late fee calculation
- **Invoice Generation**: Generate invoices for students
- **Payment Processing**: Integration with payment systems

## Service Layer

### FeeService Implementation
```csharp
public class FeeService : BaseApiService, IFeeService
{
    // Implements all CRUD operations for all fee components
    // Handles HTTP communication with API
    // Manages query parameter construction
    // Provides typed responses
    // Supports business operations
}
```

### Key Methods
- **Fee Categories**: GetAllFeeCategories, GetFeeCategory, CreateFeeCategory, UpdateFeeCategory, DeleteFeeCategory
- **Fee Templates**: GetAllFeeTemplates, GetFeeTemplate, GetFeeTemplatesByCategory, GetFeeTemplatesByType, CreateFeeTemplate, UpdateFeeTemplate, DeleteFeeTemplate
- **Student Fees**: GetStudentFees, GetStudentFee, GetStudentFeesByEnrollment, AssignFeeToStudent, UpdateStudentFee, WaiveStudentFee, DeleteStudentFee
- **Business Operations**: GetOverdueFees, GetPendingFees, CalculateLateFees, GenerateInvoices
- **Validation**: ValidateFeeTemplate, CanDeleteFeeCategory, CanDeleteFeeTemplate

## Business Logic

### 1. Fee Category Management
- **Purpose**: Organize fees into logical categories
- **Mandatory Categories**: Required fees that must be paid
- **Display Order**: Control the order of category display
- **Status Management**: Active/Inactive status for categories

### 2. Fee Template Management
- **Purpose**: Define standard fee structures
- **Fee Types**: Categorize fees by type (Tuition, Registration, Materials, Exam, Certificate, Late Fee, Deposit, Other)
- **Amount Configuration**: Set base amounts for fees
- **Currency Support**: Support multiple currencies
- **Applicability Rules**: Define which students/levels templates apply to
- **Discount Configuration**: Early payment and sibling discounts
- **Late Fee Management**: Late fee calculation and application rules

### 3. Student Fee Assignment
- **Purpose**: Assign specific fees to individual students
- **Template-Based**: Use fee templates for consistent fee structures
- **Custom Amounts**: Override template amounts when necessary
- **Payment Tracking**: Track payments and outstanding amounts
- **Status Management**: Comprehensive status tracking
- **Discount Application**: Apply various types of discounts
- **Fee Waiver**: Waive fees with proper documentation

### 4. Payment Processing
- **Payment Tracking**: Track partial and full payments
- **Outstanding Amounts**: Calculate remaining amounts due
- **Invoice Management**: Generate and manage invoices
- **Payment History**: Maintain complete payment history

### 5. Late Fee Management
- **Automatic Calculation**: Calculate late fees based on due dates
- **Late Fee Rules**: Configurable late fee amounts and timing
- **Grace Periods**: Allow grace periods before late fees apply
- **Status Updates**: Automatically update fee status to overdue

## Validation Rules

### Business Validation
- Fee category names must be unique
- Fee template names must be unique
- Student fees must reference valid students
- Fee amounts must be positive
- Due dates must be valid
- Currency codes must be valid
- Fee types must be from predefined list

### Form Validation
- Required fields enforced
- String length limits
- Numeric range validations
- Date validations
- Currency format validation
- Client-side real-time validation
- Server-side validation as backup

## Integration Points

- **Student Management**: Integration with student profiles
- **Enrollment Management**: Integration with enrollment system
- **Course Management**: Integration with course and section management
- **Payment Systems**: Integration with payment processing
- **Academic Records**: Integration with academic performance tracking
- **Reporting**: Financial reporting and analytics
- **Invoice Systems**: Integration with invoice generation

## Performance Considerations

- **Pagination**: Large fee catalogs handled efficiently
- **Caching**: Consider caching fee templates for frequent access
- **Indexing**: Database indexes on frequently queried fields
- **Lazy Loading**: Partial views for better page load times
- **Search Optimization**: Efficient filtering and search
- **Bulk Operations**: Support for bulk fee operations

## Security Considerations

- **Authentication**: All endpoints require valid JWT token
- **Authorization**: Role-based permissions enforced
- **Input Validation**: Server-side validation on all DTOs
- **CSRF Protection**: Anti-forgery tokens on all forms
- **Audit Trail**: CreatedBy/ModifiedBy tracking on all changes
- **Data Protection**: Soft delete for fee history preservation
- **Financial Security**: Sensitive financial information properly protected

## Reporting and Analytics

- Fee collection statistics
- Payment trends and patterns
- Overdue fee analysis
- Revenue reporting
- Student payment behavior
- Fee category performance
- Template utilization metrics

## Future Enhancements

1. **Advanced Payment Processing**: Integration with payment gateways
2. **Automated Fee Calculation**: Automatic fee calculation based on rules
3. **Bulk Operations**: Mass fee operations and updates
4. **Payment Plans**: Installment payment plans
5. **Financial Reporting**: Advanced financial reporting and analytics
6. **Mobile Payments**: Mobile payment integration
7. **Fee Templates**: Advanced template management
8. **Automated Invoicing**: Automated invoice generation and sending
9. **Payment Reminders**: Automated payment reminder system
10. **Financial Dashboard**: Comprehensive financial dashboard

## Usage Examples

### Creating a Fee Category
1. Navigate to Fee Categories management
2. Click "Add Fee Category" button
3. Fill in category details:
   - Category Name: "Tuition Fees"
   - Category Code: "TUITION"
   - Description: "Regular tuition fees"
   - Mandatory: Yes
   - Display Order: 1
4. Save category

### Creating a Fee Template
1. Navigate to Fee Templates management
2. Click "Add Fee Template" button
3. Fill in template details:
   - Template Name: "Chinese Level 1 Tuition"
   - Category: Select "Tuition Fees"
   - Fee Type: "Tuition"
   - Base Amount: 500.00
   - Currency: "USD"
   - Applicable To: "All"
   - Due Days After Enrollment: 7
   - Late Fee Days: 7
   - Late Fee Amount: 25.00
4. Save template

### Assigning Fee to Student
1. Navigate to Student Fees management
2. Click "Assign Fee" button
3. Fill in assignment details:
   - Student: Select from existing students
   - Fee Template: Select "Chinese Level 1 Tuition"
   - Custom Amount: 500.00 (or leave blank to use template amount)
   - Due Date: Set appropriate due date
   - Notes: Additional information
4. System calculates final amount
5. Save assignment

### Waiving a Student Fee
1. Select student fee from list
2. Click "Waive" action
3. Fill in waiver details:
   - Waive Reason: "Financial hardship"
   - Waived Reason: "Student eligible for financial assistance"
4. Submit waiver
5. Fee status updated to "Waived"

## API Response Examples

### GetFeeCategories Response
```json
[
  {
    "categoryId": "guid-here",
    "categoryName": "Tuition Fees",
    "categoryCode": "TUITION",
    "description": "Regular tuition fees",
    "isMandatory": true,
    "displayOrder": 1,
    "status": "Active",
    "createdBy": "admin",
    "createDate": "2023-01-01T00:00:00Z"
  }
]
```

### GetFeeTemplates Response
```json
[
  {
    "templateId": "guid-here",
    "categoryId": "category-guid-here",
    "categoryName": "Tuition Fees",
    "templateName": "Chinese Level 1 Tuition",
    "feeType": "Tuition",
    "baseAmount": 500.00,
    "currency": "USD",
    "applicableTo": "All",
    "dueDaysAfterEnrollment": 7,
    "lateFeeDays": 7,
    "lateFeeAmount": 25.00,
    "earlyPaymentDiscountPercent": 5.00,
    "effectiveDate": "2023-01-01",
    "expiryDate": null,
    "status": "Active",
    "createdBy": "admin",
    "createDate": "2023-01-01T00:00:00Z"
  }
]
```

### GetStudentFees Response
```json
[
  {
    "studentFeeId": "guid-here",
    "studentId": "student-guid-here",
    "studentName": "John Doe",
    "studentCode": "S001",
    "templateId": "template-guid-here",
    "templateName": "Chinese Level 1 Tuition",
    "categoryName": "Tuition Fees",
    "enrollmentId": "enrollment-guid-here",
    "courseName": "Chinese Level 1",
    "sectionCode": "CHN101-A",
    "feeName": "Chinese Level 1 Tuition",
    "feeType": "Tuition",
    "originalAmount": 500.00,
    "discountAmount": 0.00,
    "discountReason": null,
    "finalAmount": 500.00,
    "currency": "USD",
    "dueDate": "2023-09-08",
    "gracePeriodDays": 0,
    "lateFeeApplied": 0.00,
    "status": "Pending",
    "amountPaid": 0.00,
    "amountOutstanding": 500.00,
    "invoiceNumber": null,
    "notes": null,
    "createdBy": "admin",
    "createDate": "2023-09-01T00:00:00Z"
  }
]
```

### GetOverdueFees Response
```json
[
  {
    "studentFeeId": "guid-here",
    "studentName": "John Doe",
    "studentCode": "S001",
    "feeName": "Chinese Level 1 Tuition",
    "finalAmount": 500.00,
    "dueDate": "2023-09-08",
    "daysOverdue": 15,
    "lateFeeApplied": 25.00,
    "status": "Overdue"
  }
]
```

This comprehensive Fee management system provides a robust foundation for financial administration within the Hong Wen education platform, supporting all aspects of fee management from category creation to payment processing and financial reporting.
