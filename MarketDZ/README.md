MarketDZ
A cross-platform marketplace application built with .NET MAUI and Firebase Realtime Database.
Overview
MarketDZ is a modern marketplace app that allows users to buy, sell, rent items, and post jobs and services. The app provides a seamless experience across iOS, Android, macOS, and Windows platforms, all powered by a Firebase backend.
Features
User Authentication

Email-based registration and login
Secure password management
Persistent sessions across app restarts

Marketplace Functionality

Browse items across multiple categories
Post items for sale with images
List rental properties and items
Post job listings with detailed information
Offer services with rate and availability details
Search and filter by location, price, and category

Item Management

Create, edit, and delete listings
Upload and manage item photos
Mark items as sold, rented, or unavailable

Location Features

Find items near your current location
Browse items by state/province
Sort items by distance

Messaging System

Contact sellers directly through the app
Manage conversations with potential buyers
Receive notifications for new messages

Technical Details
Architecture

Built with .NET MAUI for cross-platform compatibility
MVVM architecture using MVVM Community Toolkit
Repository pattern for data access
Firebase Realtime Database for backend storage

Core Technologies

C# / .NET 8
.NET MAUI
Firebase Realtime Database
MVVM Community Toolkit
Dependency Injection

Key Components

Firebase Authentication
Firebase Storage (for images)
Geolocation services
Real-time messaging
Media capture and upload

Getting Started
Prerequisites

Visual Studio 2022 or later with .NET MAUI workload installed
A Firebase account and project
Firebase Realtime Database instance

Setup

Clone the repository
Set up your Firebase project and add the appropriate connection string in MauiProgram.cs
Ensure Firebase Realtime Database rules are configured correctly
Build and run the application

Firebase Database Rules
The app requires specific Firebase Realtime Database rules to function correctly. Here's a sample configuration:
jsonCopy{
  "rules": {
    ".read": false,
    ".write": false,
    
    "test": {
      ".read": true,
      ".write": true
    },
    
    "connectionTest": {
      ".read": true,
      ".write": true
    },
    
    "categories": {
      ".read": true,
      ".write": true
    },
    
    "users": {
      ".read": true,
      ".write": true
    },
    
    "items": {
      ".read": true,
      ".write": true
    },
    
    "messages": {
      ".read": true,
      ".write": true
    }
  }
}
Contributing
Contributions are welcome! Please feel free to submit a Pull Request.
License
This project is licensed under the MIT License - see the LICENSE file for details.
Acknowledgements

.NET MAUI Documentation
Firebase Documentation
MVVM Community Toolkit

Contact
For any inquiries, please open an issue in the repository.
