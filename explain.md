# PharmaChain - Comprehensive Project Documentation

## 📋 Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture & Technology Stack](#architecture--technology-stack)
3. [Project Structure](#project-structure)
4. [Database Schema & Models](#database-schema--models)
5. [Controllers & Business Logic](#controllers--business-logic)
6. [Views & User Interface](#views--user-interface)
7. [Authentication & Authorization](#authentication--authorization)
8. [Security Features](#security-features)
9. [Configuration & Setup](#configuration--setup)
10. [File-by-File Breakdown](#file-by-file-breakdown)
11. [Data Flow & User Journey](#data-flow--user-journey)
12. [Key Features Implementation](#key-features-implementation)

---

## 🏥 Project Overview

**PharmaChain** is a comprehensive pharmaceutical supply chain management system built with ASP.NET Core MVC. It facilitates the management of medicines, orders, and inventory across different stakeholders in the pharmaceutical industry, including manufacturers, suppliers, customers, and administrators.

### Core Purpose
- **Streamline pharmaceutical supply chain operations**
- **Ensure proper tracking of medicines from production to delivery**
- **Maintain inventory levels and manage orders efficiently**
- **Provide role-based access control for different stakeholders**
- **Enable real-time monitoring and management of pharmaceutical operations**

---

## 🏗️ Architecture & Technology Stack

### Technology Stack
- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, jQuery, DataTables
- **Architecture Pattern**: Model-View-Controller (MVC)
- **ORM**: Entity Framework Core with Code-First approach

### Architecture Components
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Presentation  │    │   Business      │    │      Data       │
│     Layer       │    │     Layer       │    │     Layer       │
│                 │    │                 │    │                 │
│ • Views         │◄──►│ • Controllers   │◄──►│ • Models        │
│ • ViewModels    │    │ • Services      │    │ • DbContext     │
│ • Static Files  │    │ • Middleware    │    │ • Migrations    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

---

## 📁 Project Structure

```
PharmaChain/
├── Controllers/              # MVC Controllers
│   ├── AccountController.cs     # Authentication & Profile Management
│   ├── AdminController.cs       # System Administration
│   ├── BaseController.cs        # Shared Controller Logic
│   ├── CustomerController.cs    # Customer Operations
│   ├── HomeController.cs        # Landing Page
│   ├── ManufacturerController.cs # Medicine & Order Management
│   └── SupplierController.cs    # Inventory & Order Fulfillment
├── Data/                    # Data Access Layer
│   └── ApplicationDbContext.cs  # Entity Framework Context
├── Migrations/              # Database Migrations
│   ├── 20250914132820_InitialCreate.cs
│   ├── 20250918160859_AddAdminRoleAndCompanyName.cs
│   └── ApplicationDbContextModelSnapshot.cs
├── Middleware/              # Custom Middleware
│   └── NoCacheMiddleware.cs     # Cache Control for Security
├── Models/                  # Data Models
│   ├── ApplicationUser.cs       # User Entity (extends IdentityUser)
│   ├── ErrorViewModel.cs        # Error Handling
│   ├── Inventory.cs             # Inventory Management
│   ├── Medicine.cs              # Medicine Entity
│   └── Order.cs                 # Order Entity
├── ViewModels/              # View-Specific Models
│   ├── AdminDashboardViewModel.cs
│   ├── CustomerDashboardViewModel.cs
│   ├── EditProfileViewModel.cs
│   ├── EditUserViewModel.cs
│   ├── LoginViewModel.cs
│   ├── ManufacturerDashboardViewModel.cs
│   ├── MedicineViewModel.cs
│   ├── ProfileViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── SearchMedicinesViewModel.cs
│   └── SupplierDashboardViewModel.cs
├── Views/                   # Razor Views
│   ├── Account/                 # Authentication Views
│   ├── Admin/                   # Admin Dashboard Views
│   ├── Customer/                # Customer Views
│   ├── Home/                    # Landing Page Views
│   ├── Manufacturer/            # Manufacturer Views
│   ├── Shared/                  # Shared Layouts & Components
│   └── Supplier/                # Supplier Views
├── wwwroot/                 # Static Files
│   ├── css/                     # Custom Stylesheets
│   ├── js/                      # JavaScript Files
│   └── lib/                     # Third-party Libraries
├── Program.cs               # Application Entry Point
├── appsettings.json         # Configuration
└── PharmaChain.csproj       # Project File
```

---

## 🗄️ Database Schema & Models

### Core Entities

#### 1. ApplicationUser (extends IdentityUser)
```csharp
public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }           // Customer name
    public string Role { get; set; }            // User role
    public string? CompanyName { get; set; }    // Company name for businesses
    public bool IsApproved { get; set; }        // Approval status
    public DateTime CreatedAt { get; set; }     // Registration date
    
    // Computed property for display
    public string DisplayName => Role switch
    {
        "Customer" => Name ?? Email ?? "Unknown",
        "Manufacturer" or "Supplier" or "Admin" => CompanyName ?? Email ?? "Unknown",
        _ => Name ?? CompanyName ?? Email ?? "Unknown"
    };
}
```

**Purpose**: Central user entity that handles all user types with role-based properties.

#### 2. Medicine
```csharp
public class Medicine
{
    public int MedicineID { get; set; }
    public string Name { get; set; }            // Medicine name
    public string BatchNo { get; set; }         // Unique batch number
    public DateTime ExpiryDate { get; set; }    // Expiry date
    public int Quantity { get; set; }           // Available quantity
    public decimal Price { get; set; }          // Unit price
    public string ManufacturerID { get; set; }  // Foreign key to manufacturer
    public ApplicationUser? Manufacturer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Business logic
    public const int LOW_STOCK_THRESHOLD = 10;
    public bool IsLowStock => Quantity <= LOW_STOCK_THRESHOLD;
}
```

**Purpose**: Represents pharmaceutical products with batch tracking and inventory management.

#### 3. Order
```csharp
public class Order
{
    public int OrderID { get; set; }
    public string CustomerID { get; set; }      // Foreign key to customer
    public int MedicineID { get; set; }         // Foreign key to medicine
    public int Quantity { get; set; }           // Ordered quantity
    public DateTime OrderDate { get; set; }     // Order placement date
    public OrderStatus Status { get; set; }     // Order status
    public decimal TotalAmount { get; set; }    // Calculated total
    public ApplicationUser? Customer { get; set; }
    public Medicine? Medicine { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}

public enum OrderStatus
{
    Pending,        // Waiting for approval
    Approved,       // Approved by manufacturer/supplier
    Delivered,      // Delivered to customer
    Rejected        // Rejected by manufacturer/supplier
}
```

**Purpose**: Manages order lifecycle from placement to delivery with status tracking.

#### 4. Inventory
```csharp
public class Inventory
{
    public int InventoryID { get; set; }
    public string UserID { get; set; }          // Foreign key to supplier
    public int MedicineID { get; set; }         // Foreign key to medicine
    public int Quantity { get; set; }           // Stock quantity
    public ApplicationUser? User { get; set; }
    public Medicine? Medicine { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Business logic
    public const int LOW_STOCK_THRESHOLD = 5;
    public bool IsLowStock => Quantity <= LOW_STOCK_THRESHOLD;
}
```

**Purpose**: Tracks supplier inventory levels for each medicine.

### Database Relationships
```
ApplicationUser (1) ──→ (Many) Medicine [ManufacturerID]
ApplicationUser (1) ──→ (Many) Order [CustomerID]
ApplicationUser (1) ──→ (Many) Inventory [UserID]
Medicine (1) ──→ (Many) Order [MedicineID]
Medicine (1) ──→ (Many) Inventory [MedicineID]
```

---

## 🎮 Controllers & Business Logic

### 1. BaseController
**Purpose**: Provides shared functionality for all controllers.
```csharp
public abstract class BaseController : Controller
{
    protected readonly UserManager<ApplicationUser> _userManager;
    protected readonly ApplicationDbContext _context;
    
    protected async Task<ApplicationUser?> GetCurrentUserAsync()
    protected async Task<bool> IsInRoleAsync(string role)
    protected async Task<bool> IsApprovedAsync()
    protected void SetNoCacheHeaders()
}
```

### 2. AccountController
**Purpose**: Handles authentication, registration, and profile management.

**Key Actions**:
- `Register()` - User registration with role selection
- `Login()` - User authentication
- `Logout()` - User logout with cache clearing
- `Profile()` - View user profile
- `EditProfile()` - Update user profile

**Business Logic**:
- Auto-approves Manufacturers and Admins
- Requires approval for Customers and Suppliers
- Role-based profile field display

### 3. AdminController
**Purpose**: System administration and global management.

**Key Actions**:
- `Index()` - Admin dashboard with system statistics
- `Users()` - Manage all users across roles
- `EditUser()` - Edit user details and approval status
- `Medicines()` - View and manage all medicines
- `DeleteMedicine()` - Remove medicines from system
- `Orders()` - Monitor all orders across the system

**Business Logic**:
- Full system oversight capabilities
- User approval management
- Global medicine and order monitoring

### 4. ManufacturerController
**Purpose**: Medicine creation, order management, and related user oversight.

**Key Actions**:
- `Index()` - Dashboard with medicine and order statistics
- `CreateMedicine()` - Add new medicines to catalog
- `EditMedicine()` - Update medicine details
- `Medicines()` - Manage own medicines
- `Orders()` - View and approve customer orders
- `Users()` - Manage related customers and suppliers

**Business Logic**:
- Can only manage medicines they created
- Can only see orders for their medicines
- User management filtered to related users only

### 5. SupplierController
**Purpose**: Inventory management, order fulfillment, and purchasing.

**Key Actions**:
- `Index()` - Dashboard with inventory and order statistics
- `Inventory()` - Manage stock levels
- `BuyFromManufacturer()` - Purchase medicines from manufacturers
- `SellToCustomer()` - Fulfill customer orders
- `CustomerOrders()` - View customer orders
- `Orders()` - Track all supplier orders

**Business Logic**:
- Manages inventory for multiple medicines
- Can purchase from any manufacturer
- Fulfills orders from available inventory

### 6. CustomerController
**Purpose**: Medicine browsing, order placement, and order tracking.

**Key Actions**:
- `Index()` - Customer dashboard
- `SearchMedicines()` - Browse and search medicines
- `PlaceOrder()` - Create new orders
- `Orders()` - View order history and status

**Business Logic**:
- Can only place orders for available medicines
- Can only view their own orders
- Limited to browsing and purchasing functionality

### 7. HomeController
**Purpose**: Landing page and general information.

**Key Actions**:
- `Index()` - Public landing page with authentication options
- `Privacy()` - Privacy policy page

---

## 🎨 Views & User Interface

### View Structure by Role

#### Admin Views (`/Views/Admin/`)
- **Index.cshtml** - Dashboard with system statistics and quick actions
- **Users.cshtml** - User management table with approval controls
- **EditUser.cshtml** - User editing form
- **Medicines.cshtml** - Medicine management table
- **Orders.cshtml** - Order monitoring table

#### Manufacturer Views (`/Views/Manufacturer/`)
- **Index.cshtml** - Dashboard with medicine and order metrics
- **CreateMedicine.cshtml** - Medicine creation form
- **EditMedicine.cshtml** - Medicine editing form
- **Medicines.cshtml** - Own medicines management
- **Orders.cshtml** - Customer orders for their medicines
- **Users.cshtml** - Related users management

#### Supplier Views (`/Views/Supplier/`)
- **Index.cshtml** - Dashboard with inventory and order metrics
- **Inventory.cshtml** - Stock level management
- **BuyFromManufacturer.cshtml** - Purchase form
- **SellToCustomer.cshtml** - Order fulfillment form
- **CustomerOrders.cshtml** - Customer order management
- **Orders.cshtml** - Order tracking

#### Customer Views (`/Views/Customer/`)
- **Index.cshtml** - Customer dashboard
- **SearchMedicines.cshtml** - Medicine browsing and search
- **Orders.cshtml** - Order history and tracking

#### Account Views (`/Views/Account/`)
- **Login.cshtml** - Authentication form
- **Register.cshtml** - Registration form with role selection
- **Profile.cshtml** - User profile display
- **EditProfile.cshtml** - Profile editing form

#### Shared Views (`/Views/Shared/`)
- **_Layout.cshtml** - Main layout template
- **_ViewImports.cshtml** - Global view imports
- **_ViewStart.cshtml** - View configuration
- **Error.cshtml** - Error page template

### UI Features
- **Responsive Design**: Bootstrap 5 grid system
- **Data Tables**: jQuery DataTables for sortable, searchable tables
- **Icons**: Font Awesome icons throughout
- **Alerts**: Bootstrap alert components for messages
- **Forms**: Bootstrap form styling with validation
- **Cards**: Dashboard cards for metrics display

---

## 🔐 Authentication & Authorization

### ASP.NET Core Identity Integration
```csharp
// Program.cs configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
```

### Role-Based Access Control
- **Admin**: Full system access
- **Manufacturer**: Medicine and order management
- **Supplier**: Inventory and order fulfillment
- **Customer**: Medicine browsing and order placement

### Authorization Attributes
```csharp
[Authorize]                    // Requires authentication
[Authorize(Roles = "Admin")]   // Role-specific access
```

### User Approval System
- **Auto-approved**: Manufacturers and Admins
- **Manual approval**: Customers and Suppliers
- **Approval workflow**: Admin reviews and approves new users

---

## 🛡️ Security Features

### 1. NoCacheMiddleware
**Purpose**: Prevents back button access after logout.
```csharp
public class NoCacheMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true || 
            context.Request.Path.StartsWithSegments("/Account/Logout"))
        {
            context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            context.Response.Headers.Add("Pragma", "no-cache");
            context.Response.Headers.Add("Expires", "0");
        }
        await _next(context);
    }
}
```

### 2. Password Security
- Minimum 6 characters
- Requires uppercase, lowercase, and digits
- ASP.NET Core Identity hashing

### 3. Input Validation
- Model validation attributes
- Client-side and server-side validation
- SQL injection protection via Entity Framework

### 4. Authorization Checks
- Role-based access control
- User approval verification
- Resource ownership validation

---

## ⚙️ Configuration & Setup

### Database Configuration
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PharmaChainDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Entity Framework Setup
```csharp
// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Middleware Pipeline
```csharp
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<NoCacheMiddleware>();
```

---

## 📄 File-by-File Breakdown

### Core Application Files

#### Program.cs
**Purpose**: Application entry point and service configuration.
- Configures Entity Framework
- Sets up Identity services
- Configures middleware pipeline

#### appsettings.json
**Purpose**: Application configuration.
- Database connection strings
- Logging configuration
- Application settings

### Controllers

#### AccountController.cs
**Lines**: 194
**Purpose**: Authentication and profile management.
**Key Methods**:
- `Register()` - User registration with role-based logic
- `Login()` - Authentication with approval checking
- `Logout()` - Secure logout with cache clearing
- `Profile()` - User profile display
- `EditProfile()` - Profile updates

#### AdminController.cs
**Lines**: ~200
**Purpose**: System administration.
**Key Methods**:
- `Index()` - Dashboard with system metrics
- `Users()` - User management
- `EditUser()` - User editing
- `Medicines()` - Medicine oversight
- `Orders()` - Order monitoring

#### ManufacturerController.cs
**Lines**: ~300
**Purpose**: Medicine and order management.
**Key Methods**:
- `CreateMedicine()` - Medicine creation
- `EditMedicine()` - Medicine updates
- `Orders()` - Order approval
- `Users()` - Related user management

#### SupplierController.cs
**Lines**: ~250
**Purpose**: Inventory and order fulfillment.
**Key Methods**:
- `Inventory()` - Stock management
- `BuyFromManufacturer()` - Purchasing
- `SellToCustomer()` - Order fulfillment

#### CustomerController.cs
**Lines**: ~150
**Purpose**: Medicine browsing and ordering.
**Key Methods**:
- `SearchMedicines()` - Medicine search
- `PlaceOrder()` - Order creation
- `Orders()` - Order tracking

### Models

#### ApplicationUser.cs
**Lines**: 22
**Purpose**: User entity with role-based properties.
**Key Features**:
- Extends IdentityUser
- Role-based name display
- Approval status tracking

#### Medicine.cs
**Lines**: 42
**Purpose**: Medicine entity with business logic.
**Key Features**:
- Batch tracking
- Low stock detection
- Price and quantity management

#### Order.cs
**Lines**: 42
**Purpose**: Order entity with status tracking.
**Key Features**:
- Order lifecycle management
- Status enumeration
- Date tracking

#### Inventory.cs
**Lines**: 31
**Purpose**: Inventory tracking for suppliers.
**Key Features**:
- Stock level management
- Low stock alerts
- User-medicine relationships

### Data Layer

#### ApplicationDbContext.cs
**Lines**: 73
**Purpose**: Entity Framework context configuration.
**Key Features**:
- Entity relationships
- Index configuration
- Decimal precision settings

### ViewModels

#### AdminDashboardViewModel.cs
**Purpose**: Admin dashboard data binding.
**Properties**: TotalUsers, TotalMedicines, TotalOrders, PendingApprovals

#### ManufacturerDashboardViewModel.cs
**Purpose**: Manufacturer dashboard data binding.
**Properties**: TotalMedicines, TotalOrders, PendingApprovals, LowStockMedicines

#### SupplierDashboardViewModel.cs
**Purpose**: Supplier dashboard data binding.
**Properties**: TotalInventory, TotalOrders, LowStockItems, PendingOrders

#### CustomerDashboardViewModel.cs
**Purpose**: Customer dashboard data binding.
**Properties**: TotalOrders, PendingOrders, DeliveredOrders

### Views

#### Shared/_Layout.cshtml
**Purpose**: Main application layout.
**Features**:
- Responsive navigation
- Role-based menu items
- User profile dropdown
- Bootstrap integration

#### Home/Index.cshtml
**Purpose**: Landing page.
**Features**:
- Authentication buttons
- Core features showcase
- Public information

---

## 🔄 Data Flow & User Journey

### 1. User Registration Flow
```
User Registration → Role Selection → Account Creation → 
Auto-approval (Manufacturer/Admin) OR Manual Approval (Customer/Supplier) → 
Dashboard Access
```

### 2. Medicine Creation Flow (Manufacturer)
```
Manufacturer Login → Create Medicine → Medicine Added to Catalog → 
Available for Suppliers to Purchase
```

### 3. Order Processing Flow
```
Customer Places Order → Manufacturer/Supplier Reviews → 
Approval/Rejection → Delivery Tracking → Order Completion
```

### 4. Inventory Management Flow (Supplier)
```
Supplier Purchases from Manufacturer → Inventory Updated → 
Customer Orders → Stock Deduction → Low Stock Alerts
```

---

## 🎯 Key Features Implementation

### 1. Role-Based Dashboard System
- **Dynamic Content**: Each role sees relevant metrics
- **Quick Actions**: Role-specific action buttons
- **Real-time Data**: Live statistics and alerts

### 2. Medicine Batch Tracking
- **Unique Batch Numbers**: Ensures pharmaceutical compliance
- **Expiry Date Monitoring**: Prevents expired medicine sales
- **Quantity Management**: Real-time stock tracking

### 3. Order Lifecycle Management
- **Status Tracking**: Pending → Approved → Delivered
- **Date Tracking**: Order, approval, and delivery timestamps
- **Rejection Handling**: Proper order rejection workflow

### 4. Inventory Management
- **Multi-supplier Support**: Suppliers can stock multiple medicines
- **Low Stock Alerts**: Automatic threshold-based alerts
- **Purchase Tracking**: Manufacturer-to-supplier transactions

### 5. User Management System
- **Approval Workflow**: Admin approval for new users
- **Role-based Filtering**: Users see only relevant contacts
- **Profile Management**: Role-appropriate profile fields

### 6. Security Implementation
- **Cache Control**: Prevents back button access after logout
- **Input Validation**: Comprehensive form validation
- **Authorization**: Role-based access control throughout

---

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Setup Steps
1. Clone the repository
2. Update connection string in `appsettings.json`
3. Run `dotnet restore` to install packages
4. Run `dotnet ef database update` to create database
5. Run `dotnet run` to start the application
6. Login with default admin: `admin@pharmachain.com` / `Admin@123`

### Default Credentials
- **Admin**: admin@pharmachain.com / Admin@123
- **Company**: PharmaChain Systems

---

## 🔮 Future Enhancements

### Planned Features
- Real-time notifications
- Mobile application
- Advanced reporting
- API for third-party integrations
- Barcode scanning
- Email notifications
- Advanced search capabilities

### Technical Improvements
- Unit testing implementation
- API documentation
- Performance optimization
- Caching strategies
- Logging and monitoring

---

This comprehensive documentation provides a complete understanding of the PharmaChain system, its architecture, components, and implementation details. Each file and feature has been designed to work together in a cohesive pharmaceutical supply chain management solution.
