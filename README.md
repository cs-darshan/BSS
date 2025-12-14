# BSS
# Sensore – Pressure Analysis & Visualization System

## Project Overview

Sensore is a web-based pressure analysis system that processes, analyzes, and visualizes pressure sensor data captured as 32×32 matrices. The system serves both users and administrators. It allows the import of pressure data from CSV files, enables frame-by-frame analysis, stores data in a database, and provides visualization through interactive timelines and heatmaps.

The application features an ASP.NET Core (C#) backend and a JavaScript-based frontend that uses Chart.js and HTML Canvas for visualization. It aims to identify pressure peaks, contact area, pressure distribution, and overall risk scoring to support monitoring and analysis tasks.

## Key Features

* Secure login system with role-based access (User / Admin)
* CSV import pipeline that converts raw sensor data into 32×32 pressure frames
* Backend analysis of pressure data that includes peak pressure, contact area, distribution, and risk score
* Interactive timeline graph to browse pressure frames
* Click-to-select frame functionality with real-time heatmap rendering
* Data persistence using Entity Framework Core
* Separate dashboards for users and administrators

## System Architecture

* **Backend:** ASP.NET Core Web API (C#)
* **Frontend:** HTML, JavaScript, Chart.js, Canvas
* **Database:** Entity Framework Core (relational database)
* **Authentication:** Cookie-based authentication with claims
* **Data Source:** CSV files containing pressure sensor readings

## Component Responsibility Breakdown

| Component                   | Developer |
| --------------------------- | --------- |
| Backend API & Services      | Darshan   |
| Frontend UI & Visualization | Darshan   |
| Authentication System       | Darshan   |
| CSV Import & Analysis       | Darshan   |
| Database Models             | Darshan   |
| User Backend                | Misbah    |
| Admin Frontend              | Misbah    |
| Login Testing               | Misbah    |
| Login Portal                | Misbah    |
| Documentation               | Misbah    |

## Backend Responsibilities

* Designed and implemented RESTful APIs for authentication, CSV import, timeline retrieval, and frame analysis
* Built logic to analyze pressure, calculate peak pressure, contact area, pressure distribution, and risk score
* Implemented secure session handling using claims and cookies
* Created and maintained database models for users, pressure frames, metrics, comments, and alerts
* Linked imported CSV sessions to users and ensured proper data retrieval

## Frontend Responsibilities

* Developed interactive timeline graphs using Chart.js
* Implemented frame selection through clicking to display pressure heatmaps
* Built dynamic canvas-based heatmap rendering for 32×32 matrices
* Connected frontend with backend APIs for smooth data flow

## Data Flow Summary

1. CSV files are imported and read as continuous sensor rows
2. Rows are grouped into 32×32 pressure frames
3. Each frame is analyzed for pressure metrics
4. Results are stored in the database
5. Timeline data is sent to the frontend
6. Users and admins interactively explore frames and heatmaps

## User Roles

* **User:** Can view only their own pressure sessions, timelines, and heatmaps
* **Admin:** Can select any session, analyze frames, and view aggregated pressure data

## Challenges & Lessons Learned

* Handling large CSV datasets needed careful frame parsing and validation
* Poor communication among some team members caused delays and required restarting parts of the project
* Rebuilding the system from a clean base plan ensured stability and consistency
* A solid backend structure made frontend integration much easier

## Future Improvements

* Real-time sensor streaming instead of CSV-based imports
* Better alerting and notification system
* Enhanced UI/UX for mobile devices
* Role-based analytics dashboards for clinicians
* Automated testing and CI/CD integration

## Conclusion

Sensore shows a complete full-stack implementation of a pressure analysis system. It combines backend data processing, database management, and interactive frontend visualization. Despite collaboration challenges, the final system provides a functional, extensible, and well-structured solution built from the ground up.
