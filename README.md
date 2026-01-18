# TailorNow - Premium On-Demand Tailoring Service

**TailorNow** is a comprehensive ASP.NET Core web application designed to bridge the gap between professional tailors and customers. It offers a seamless, end-to-end platform for booking appointments, managing tailoring services, and tracking commissions, all wrapped in a premium, responsive user interface.

![Live Demo](wwwroot/videos/Demo.mp4)

##  Key Features....

###  Customer Experience
- **Dynamic Booking**: A user-friendly interface to browse professional tailors and book appointments with real-time service selection.
- **Personalized Dashboard**: Track booking status and manage profile details.
- **Shop Discovery**: Explore tailor profiles, shop locations, and service offerings.

###  Tailor Module
- **Service Management**: Define and price various tailoring services.
- **Appointment Tracking**: View and manage customer bookings and shop availability.
- **Earnings Overview**: Real-time stats on total appointments and financial performance.

###  Admin Panel
- **User Management**: Full CRUD operations for Admins, Tailors, and Customers.
- **Command Center**: Holistic dashboard with statistical charts and activity monitoring.
- **Financial Reporting**: Detailed commission breakdown and earning reports for the platform.

###  Live Demo Guide
The application includes an interactive **Live Demo Guide** on the homepage, allowing public viewers to watch recorded walkthroughs and explore the system's capabilities without needing to sign up.

---

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0 (MVC)
- **Frontend**: Bootstrap 5, Vanilla CSS, JavaScript, HTML5
- **Database**: Entity Framework Core with SQL Server
- **Identity**: Microsoft Identity for robust Authentication and RBAC (Role-Based Access Control)
- **Dev-Ops**: Docker-ready for cross-platform deployment.

---

## 🏁 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (or SQL Express)

### Installation
1.  **Clone the Repository**:
    ```bash
    git clone https://github.com/tayyaba13-02/TailorrNow.git
    ```
2.  **Navigate to Project Directory**:
    ```bash
    cd TailorrNow
    ```
3.  **Update Connection String**:
    Open `appsettings.json` and update the `DefaultConnection` to match your SQL Server instance.
4.  **Database Setup**:
    Run migrations to create the database:
    ```bash
    dotnet ef database update
    ```
5.  **Run the App**:
    ```bash
    dotnet run
    ```

---


