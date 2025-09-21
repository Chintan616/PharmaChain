# PharmaChain - Pharmaceutical Supply Chain Management System

A comprehensive pharmaceutical supply chain management system built with ASP.NET Core MVC that facilitates the management of medicines, orders, and inventory across different stakeholders in the pharmaceutical industry.

## 🏥 Overview

PharmaChain is a web-based application designed to streamline pharmaceutical supply chain operations by connecting manufacturers, suppliers, and customers in a secure, role-based environment. The system ensures proper tracking of medicines from production to delivery, maintaining inventory levels, and managing orders efficiently.

## ✨ Features

### 🔐 Authentication & Authorization
- **Role-based access control** with four user types:
  - **Admin**: System administration, user management, global oversight
  - **Manufacturers**: Create and manage medicines, approve orders, manage related users
  - **Suppliers**: Manage inventory, fulfill customer orders, buy from manufacturers
  - **Customers**: Browse medicines, place orders, track order status
- **User approval system** for new registrations
- **Secure password policies** with validation
- **Profile management** with role-specific information display

### 💊 Medicine Management
- **Medicine catalog** with detailed information:
  - Name, batch number, expiry date
  - Quantity and pricing
  - Manufacturer tracking
- **Batch tracking** for pharmaceutical compliance
- **Expiry date monitoring** for inventory management

### 📦 Order Management
- **Order lifecycle tracking**:
  - Pending → Approved → Delivered
  - Order rejection capability
- **Real-time order status** updates
- **Order history** and tracking

### 📊 Inventory Management
- **Real-time inventory tracking** for suppliers
- **Stock level monitoring**
- **Inventory updates** across the supply chain

### 🎯 User Dashboards
- **Customized dashboards** for each user role with key metrics
- **Role-specific functionality** and views
- **Profile management** and user settings
- **Real-time data visualization** with cards and tables
- **Low stock alerts** and inventory monitoring

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Razor Views with Bootstrap
- **ORM**: Entity Framework Core 8.0.11

## 📋 Prerequisites

- .NET 8.0 SDK
- SQL Server (Local or Azure)
- Visual Studio 2022 or VS Code
- Git

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone <repository-url>
cd PharmaChain
```

### 2. Database Setup
1. **Update Connection String**: Modify `appsettings.json` with your SQL Server connection details:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=PharmaChainDB;User Id=your-username;Password=your-password;TrustServerCertificate=True;"
  }
}
```

2. **Run Migrations**: The application will automatically create the database schema on first run.

### 3. Run the Application
```bash
dotnet restore
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`.

## 🏗️ Project Structure

```
PharmaChain/
├── Controllers/           # MVC Controllers
│   ├── AccountController.cs
│   ├── CustomerController.cs
│   ├── ManufacturerController.cs
│   └── SupplierController.cs
├── Models/               # Data Models
│   ├── ApplicationUser.cs
│   ├── Medicine.cs
│   ├── Order.cs
│   └── Inventory.cs
├── Views/                # Razor Views
│   ├── Account/          # Authentication views
│   ├── Customer/         # Customer-specific views
│   ├── Manufacturer/     # Manufacturer-specific views
│   └── Supplier/         # Supplier-specific views
├── Data/                 # Database Context
│   └── ApplicationDbContext.cs
├── Services/             # Business Logic Services
│   └── SeedDataService.cs
├── ViewModels/           # View Models for data binding
└── wwwroot/             # Static files (CSS, JS, Images)
```

## 👥 User Roles & Permissions

### 👨‍💼 Admin
- **System administration** and global oversight
- **User management** across all roles
- **Medicine management** and catalog oversight
- **Order management** and system monitoring
- **Access to all system data** and analytics

### 🏭 Manufacturer
- **Create and manage medicine listings** with batch tracking
- **View and approve customer orders** for their medicines
- **Manage related users** (customers and suppliers who interact with their medicines)
- **Track medicine production and distribution**
- **Monitor low stock alerts** and inventory levels

### 🚚 Supplier
- **Manage inventory levels** and stock tracking
- **Fulfill customer orders** from available inventory
- **Buy medicines from manufacturers** to restock
- **Track sales and deliveries** to customers
- **Monitor low stock items** and restock alerts

### 🛒 Customer
- **Browse available medicines** with search functionality
- **Place orders** for medicines from suppliers
- **Track order status** and delivery updates
- **View order history** and past purchases

## 🔧 Configuration

### Database Configuration
The application uses Entity Framework Core with SQL Server. Update the connection string in `appsettings.json` to point to your database server.

### Identity Configuration
Password requirements and authentication settings can be modified in `Program.cs`:
- Minimum password length: 6 characters
- Requires uppercase, lowercase, and digits
- Email confirmation: Disabled (can be enabled for production)

## 🚀 Deployment

### Local Development
1. Ensure SQL Server is running
2. Update connection string in `appsettings.json`
3. Run `dotnet run` from the project directory

### Production Deployment
1. Update connection string for production database
2. Configure HTTPS and security settings
3. Deploy to Azure App Service, IIS, or your preferred hosting platform

## 📝 Application Structure

The application follows MVC architecture with the following main controllers:
- `/Account` - Authentication, registration, and profile management
- `/Admin` - System administration and global management
- `/Customer` - Customer-specific operations and medicine browsing
- `/Manufacturer` - Medicine creation, order management, and user oversight
- `/Supplier` - Inventory management, order fulfillment, and purchasing
- `/Home` - Landing page and general information

### Key Features by Controller:
- **AccountController**: Login, registration, profile management, user authentication
- **AdminController**: User management, medicine oversight, order monitoring
- **ManufacturerController**: Medicine CRUD, order approval, related user management
- **SupplierController**: Inventory management, order fulfillment, manufacturer purchasing
- **CustomerController**: Medicine search, order placement, order tracking

## 🔒 Security Features

- **Role-based authorization** for all operations
- **Password hashing** using ASP.NET Core Identity
- **HTTPS enforcement** in production
- **Input validation** and sanitization
- **SQL injection protection** through Entity Framework

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For support and questions, please contact the development team or create an issue in the repository.

## 🎯 Recent Updates

### Version 1.0 - Current Release
- ✅ **Complete role-based system** with Admin, Manufacturer, Supplier, and Customer roles
- ✅ **Streamlined user management** with role-specific user filtering
- ✅ **Enhanced dashboards** with real-time metrics and alerts
- ✅ **Profile management** with role-appropriate information display
- ✅ **Inventory tracking** with low stock monitoring
- ✅ **Order management** with complete lifecycle tracking
- ✅ **Clean, production-ready codebase** with no debugging artifacts

### Key Improvements Made:
- Removed redundant display name fields in favor of company name
- Streamlined navigation by removing duplicate links
- Enhanced manufacturer user management to show only relevant users
- Improved dashboard layouts with better responsive design
- Cleaned up statistics functionality for better focus
- Optimized user experience with role-specific interfaces

## 🔮 Future Enhancements

- [ ] Real-time notifications and alerts
- [ ] Mobile application for on-the-go management
- [ ] Advanced reporting and analytics dashboard
- [ ] Integration with external pharmacy systems
- [ ] Barcode scanning for medicine tracking
- [ ] API for third-party integrations
- [ ] Email notifications for order status updates
- [ ] Advanced search and filtering capabilities

---

**PharmaChain** - Streamlining pharmaceutical supply chains for better healthcare delivery.
