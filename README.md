# 🎬 MVC Movie Application

A Movie Management Web Application built using ASP.NET MVC that allows users to browse, purchase, rent, comment on movies, and manage personal watchlists. The system also includes admin features for managing movies, actors, coupons, and notifications.

This project demonstrates the Model–View–Controller (MVC) design pattern with multiple modules such as Movies, Rentals, Purchases, Comments, Actors, Watchlists, and Coupons.
  
## 🚀 Features

### 🎥 Movie Management 
- Add, edit, and delete movies  
- View movie details  
- Manage movie actors
 
### 🛒 Movie Purchase
- Users can purchase movies  
- Purchase history tracking  

### 📺 Movie Rental
- Rent movies for a limited period  
- Rental history tracking
  
### 💬 Movie Comments
- Users can comment on movies  
- Display comments under each movie  

### ⭐ Watchlist
- Users can add movies to their watchlist  
- Manage personal watchlist  

### 🎟️ Discount Coupons
- Apply discount coupons when purchasing or renting movies
- Supports loyalty rewards and birthday special discounts  

### 🔔 Notifications
- System notifications for movie purchases and activities  

### 👤 User Profiles
- Users can manage their profiles   
- View user activity  

### 🧑‍💼 Admin Dashboard
- Admin panel to manage movies, users, and other system data

## 🏗️ Project Architecture
The application follows the ASP.NET MVC Architecture:  

&nbsp;&nbsp;&nbsp;MVC Pattern  
&nbsp;&nbsp;&nbsp;│  
&nbsp;&nbsp;&nbsp;├── Models  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Handles application data and business logic  
&nbsp;&nbsp;&nbsp;│  
&nbsp;&nbsp;&nbsp;├── Views  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Responsible for the user interface (UI)  
&nbsp;&nbsp;&nbsp;│  
&nbsp;&nbsp;&nbsp;└── Controllers  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Handles HTTP requests and application flow  

## 📂 Project Structure

&nbsp;&nbsp;&nbsp;MVC-Movie  
&nbsp;&nbsp;&nbsp;│  
&nbsp;&nbsp;&nbsp;├── Controllers  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── AdminDashboardController.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── CouponsController.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── HomeController.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MovieActorsController.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MovieCommentsController.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MoviePurchasesController.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MovieRentalsController.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MoviesController.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── NotificationsController.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── UserProfilesController.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;└── WatchListsController.cs  
&nbsp;&nbsp;&nbsp;│  
&nbsp;&nbsp;&nbsp;├── Models  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── AdminDashboardVM.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── Coupon.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|── Enum.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── ErrorViewModel.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── Movie.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MovieActor.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MovieComment.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MoviePurchase.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MovieRental.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── Notification.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── UserProfile.cs  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;└── WatchList.cs  
&nbsp;&nbsp;&nbsp;│  
&nbsp;&nbsp;&nbsp;├── Views  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── AdminDashboard  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── Coupons  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── Home  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MovieActors  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MovieComments  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MoviePurchases  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── MovieRentals  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── Movies  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── Notifications  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── Shared  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;├── UserProfiles  
&nbsp;&nbsp;&nbsp;│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;└── WatchLists  
&nbsp;&nbsp;&nbsp;│  
&nbsp;&nbsp;&nbsp;└── _ViewImports.cshtml  

## 🛠️ Technologies Used  
- ASP.NET MVC  
- C#  
- Entity Framework  
- Razor Views  
- SQL Server  
- Bootstrap  
- Visual Studio

## ⚙️ Installation Guide
### 1️⃣ Clone the Repository  
	git clone https://github.com/arsalanhabib01/MVC-Movie.git
### 2️⃣ Open Project  
Open the solution file in Visual Studio  
 
&nbsp;&nbsp;&nbsp;MVC-Movie.sln  

### 3️⃣ Configure Database  
Update the connection string inside:  
  
&nbsp;&nbsp;&nbsp;appsettings.json  
  
or  
  
&nbsp;&nbsp;&nbsp;Web.config  
  
(depending on project configuration)  

### 4️⃣ Run Database Migration (if applicable)  
&nbsp;&nbsp;&nbsp;Update-Database  

### 5️⃣ Run the Application  
Press:  
  
&nbsp;&nbsp;&nbsp;F5  
  
or  
  
&nbsp;&nbsp;&nbsp;Ctrl + F5  

## 🤝 Contributing  
Contributions are welcome.   
1. Fork the repository  
2. Create a feature branch  
3. Commit your changes  
4. Push to GitHub  
5. Submit a Pull Request  
  
## 👤 Author  
### **Arsalan Habib**  
  
#### GitHub:  
	https://github.com/arsalanhabib01  
