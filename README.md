# HongWen School Management System (APP)

## 📋 Overview

HongWen APP is a comprehensive ASP.NET Core MVC web application designed for managing school operations. It provides a modern, responsive interface for administrators, teachers, and staff to manage students, courses, assessments, enrollments, and other educational activities.

## 🏗️ System Architecture

### Technology Stack
- **Framework**: ASP.NET Core 6.0+ MVC
- **Frontend**: Bootstrap 5, jQuery, JavaScript
- **UI Components**: Boxicons, Custom CSS
- **Authentication**: Custom Authentication Service with JWT
- **Database**: Entity Framework Core (via API)
- **Logging**: Serilog
- **Dependency Injection**: Built-in ASP.NET Core DI

### Architecture Pattern
- **MVC Pattern**: Model-View-Controller separation
- **Service Layer**: Business logic abstraction
- **Repository Pattern**: Data access abstraction (via API)
- **DTO Pattern**: Data Transfer Objects for API communication

## 📁 Project Structure

```
HongWen_APP/
├── 📁 Controllers/                 # MVC Controllers
│   ├── AccountController.cs       # Account management
│   ├── AssessmentController.cs    # Assessment CRUD operations
│   ├── ClassroomController.cs     # Classroom management
│   ├── ClassSectionController.cs  # Class section management
│   ├── CompanyController.cs       # Company/school management
│   ├── CourseController.cs        # Course management
│   ├── EnrollmentController.cs    # Student enrollment
│   ├── FeeCategoryController.cs   # Fee category management
│   ├── FeeTemplateController.cs   # Fee template management
│   ├── HomeController.cs          # Dashboard and home
│   ├── IdentityController.cs      # Identity management
│   ├── LevelController.cs          # Academic level management
│   ├── LoginController.cs         # Authentication
│   ├── StudentController.cs       # Student management
│   ├── StudentFeeController.cs     # Student fee management
│   ├── TeacherController.cs       # Teacher management
│   ├── TermController.cs           # Academic term management
│   └── WaitlistController.cs      # Waitlist management
│
├── 📁 Models/                      # Data Models and DTOs
│   ├── 📁 AssessmentModel/         # Assessment-related models
│   │   └── 📁 DTOs/
│   │       ├── AssessmentDTOs.cs  # Assessment DTOs
│   │       └── ListAssessmentDTOs.cs
│   ├── 📁 ClassroomModel/          # Classroom models
│   ├── 📁 ClassSectionModel/       # Class section models
│   ├── 📁 CompanyModel/           # Company models
│   ├── 📁 CourseModel/            # Course models
│   ├── 📁 EnrollmentModel/        # Enrollment models
│   ├── 📁 FeeModel/               # Fee-related models
│   ├── 📁 LevelModel/             # Academic level models
│   ├── 📁 LoginModel.cs           # Authentication models
│   ├── 📁 PaginationModel.cs      # Pagination models
│   ├── 📁 StudentModel/           # Student models
│   ├── 📁 TeacherModel/           # Teacher models
│   ├── 📁 TermModel/              # Academic term models
│   └── 📁 WaitlistModel/         # Waitlist models
│
├── 📁 Services/                   # Business Logic Services
│   ├── AssessmentService.cs       # Assessment business logic
│   ├── AuthenticationService.cs  # Authentication & authorization
│   ├── BaseApiService.cs          # Base API communication
│   ├── ClassroomService.cs        # Classroom business logic
│   ├── ClassSectionService.cs     # Class section business logic
│   ├── CompanyService.cs          # Company business logic
│   ├── CourseService.cs           # Course business logic
│   ├── EnrollmentService.cs       # Enrollment business logic
│   ├── FeeService.cs              # Fee business logic
│   ├── LevelService.cs            # Level business logic
│   ├── StudentService.cs          # Student business logic
│   ├── TeacherService.cs          # Teacher business logic
│   ├── TermService.cs             # Term business logic
│   └── WaitlistService.cs         # Waitlist business logic
│
├── 📁 Views/                      # Razor Views
│   ├── 📁 Assessment/             # Assessment views
│   │   ├── Index.cshtml           # Main assessment listing
│   │   ├── _ListAssessments.cshtml # Assessment list partial
│   │   ├── _addAssessment.cshtml  # Add assessment modal
│   │   ├── _editAssessment.cshtml # Edit assessment modal
│   │   ├── _detailsAssessment.cshtml # Assessment details modal
│   │   ├── _duplicateAssessment.cshtml # Duplicate assessment modal
│   │   └── _deleteAssessment.cshtml # Delete confirmation modal
│   ├── 📁 Classroom/              # Classroom views
│   ├── 📁 ClassSection/           # Class section views
│   ├── 📁 Company/                # Company views
│   ├── 📁 Course/                 # Course views
│   ├── 📁 Enrollment/             # Enrollment views
│   ├── 📁 Fee/                    # Fee views
│   ├── 📁 Home/                   # Dashboard views
│   ├── 📁 Level/                  # Level views
│   ├── 📁 Login/                  # Authentication views
│   ├── 📁 Shared/                 # Shared views and layouts
│   │   ├── _Layout.cshtml         # Main layout
│   │   ├── _NavigationMenu.cshtml # Navigation menu
│   │   ├── _ViewImports.cshtml    # Global view imports
│   │   └── _pageList.cshtml       # Pagination partial
│   ├── 📁 Student/                # Student views
│   ├── 📁 Teacher/                # Teacher views
│   ├── 📁 Term/                   # Term views
│   └── 📁 Waitlist/               # Waitlist views
│
├── 📁 Helpers/                     # Utility Helpers
│   ├── HtmlHelpers.cs              # Custom HTML helpers
│   ├── ReturnHelper.cs             # API response helpers
│   └── UnifiedRequestHandler.cs    # Request handling utilities
│
├── 📁 Middleware/                  # Custom Middleware
│   └── CustomAuthenticationMiddleware.cs
│
├── 📁 TagHelpers/                  # Custom Tag Helpers
│   └── PermissionTagHelper.cs      # Permission-based UI rendering
│
├── 📁 wwwroot/                     # Static Files
│   ├── 📁 css/                     # Stylesheets
│   ├── 📁 js/                      # JavaScript files
│   ├── 📁 scss/                    # SCSS source files
│   └── 📁 images/                  # Images and assets
│
├── 📁 Docs/                         # Documentation
│   ├── Assessment_Management_Documentation.md
│   ├── Classroom_Management_Documentation.md
│   ├── Class_Sections_Management_Documentation.md
│   ├── Courses_Management_Documentation.md
│   ├── Enrollment_Management_Documentation.md
│   ├── Fee_Management_Documentation.md
│   ├── Levels_Management_Documentation.md
│   ├── Students_Management_Documentation.md
│   ├── Teachers_Management_Documentation.md
│   ├── Terms_Management_Documentation.md
│   └── Waitlist_Management_Documentation.md
│
├── 📄 Program.cs                   # Application entry point
├── 📄 appsettings.json             # Application configuration
├── 📄 appsettings.Development.json # Development configuration
├── 📄 hongWenAPP.csproj            # Project file
└── 📄 README.md                    # This file
```

## 🔧 Core Features

### 1. **Authentication & Authorization**
- Custom JWT-based authentication
- Role-based access control
- Permission-based UI rendering
- Secure session management

### 2. **Student Management**
- Complete student profiles
- Academic records tracking
- Enrollment management
- Fee management

### 3. **Academic Management**
- Course management
- Class section organization
- Assessment creation and tracking
- Grade management
- Academic term management

### 4. **Teacher Management**
- Teacher profiles and assignments
- Class section assignments
- Performance tracking

### 5. **Assessment System**
- Multiple assessment types (Quiz, Exam, Assignment, etc.)
- Weight-based grading system
- Due date management
- Assessment duplication
- Grade validation

### 6. **Fee Management**
- Fee category management
- Fee template creation
- Student fee tracking
- Payment processing

### 7. **Classroom Management**
- Classroom allocation
- Capacity management
- Equipment tracking
- Facility management

## 🚀 Getting Started

### Prerequisites
- .NET 6.0 or later
- Visual Studio 2022 or VS Code
- SQL Server (for API backend)
- HongWen API running

### Installation

1. **Clone the repository**
   ```bash
   git clone [repository-url]
   cd HongWen_APP
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure settings**
   - Update `appsettings.json` with your API endpoints
   - Configure authentication settings
   - Set up logging preferences

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - Navigate to `https://localhost:5001` (or configured port)
   - Login with valid credentials

## 🔐 Security Features

### Authentication
- JWT token-based authentication
- Secure password handling
- Session timeout management
- Multi-factor authentication support

### Authorization
- Role-based access control (RBAC)
- Permission-based UI rendering
- API endpoint protection
- Data access restrictions

### Data Protection
- Input validation and sanitization
- SQL injection prevention
- XSS protection
- CSRF token validation

## 📊 UI/UX Features

### Responsive Design
- Mobile-first approach
- Bootstrap 5 framework
- Custom CSS components
- Cross-browser compatibility

### User Experience
- Intuitive navigation
- Modal-based forms
- Real-time validation
- Loading states and feedback
- Searchable dropdowns
- Pagination support

### Custom Components
- Searchable dropdowns
- Modal popups
- Data tables with sorting
- Form validation
- Notification system

## 🔌 API Integration

### Service Layer
- `BaseApiService` for common API operations
- Individual services for each module
- Error handling and retry logic
- Response caching

### Data Transfer Objects (DTOs)
- Separate DTOs for each module
- Validation attributes
- Mapping between API and UI models

## 📝 Development Guidelines

### Code Structure
- Follow MVC pattern strictly
- Use dependency injection
- Implement proper error handling
- Write clean, readable code

### Naming Conventions
- Controllers: `[Module]Controller`
- Services: `[Module]Service`
- Models: `[Module]Model` folder
- Views: `[Module]/` folder
- DTOs: `[Module]DTOs.cs`

### Best Practices
- Use async/await for API calls
- Implement proper logging
- Follow SOLID principles
- Write unit tests
- Document public methods

## 🐛 Troubleshooting

### Common Issues

1. **API Connection Issues**
   - Check API endpoint configuration
   - Verify network connectivity
   - Check API authentication

2. **Authentication Problems**
   - Verify JWT token validity
   - Check session timeout
   - Validate user permissions

3. **UI Rendering Issues**
   - Check browser console for errors
   - Verify CSS/JS file loading
   - Check responsive design breakpoints

### Debug Mode
- Enable detailed error pages in development
- Use browser developer tools
- Check application logs
- Monitor network requests

## 📈 Performance Optimization

### Frontend
- Minify CSS and JavaScript
- Optimize images
- Use CDN for static assets
- Implement lazy loading

### Backend
- Use async operations
- Implement caching strategies
- Optimize database queries
- Use compression middleware

## 🔄 Deployment

### Production Deployment
1. Configure production settings
2. Set up reverse proxy (IIS/Nginx)
3. Configure SSL certificates
4. Set up monitoring and logging
5. Implement backup strategies

### Environment Configuration
- Development: Local development
- Staging: Pre-production testing
- Production: Live environment

## 📞 Support

For technical support or questions:
- Check the documentation in `/Docs` folder
- Review the code comments
- Contact the development team

## 📄 License

[Add your license information here]

---

**Last Updated**: January 2025
**Version**: 1.0.0
**Maintainer**: Development Team
