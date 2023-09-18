# WebCrawler

# Step 1: Installing Necessary Software
**SQL Server**: You will need to have SQL Server installed on your computer. Follow the installation instructions on the website.

# Step 2: Configuring the Application
**Configuring the Connection String**: In the configuration file of the application (`appsettings.json`), there is a section named "ConnectionStrings". Here, the "DbConnectionString" is already configured to connect to the database that will be created on your SQL Server. You don't need to change anything here. 

# Step 3: Running the Application
Once the SQL Server is installed, you can run the application. The application is configured to automatically create the database named "CrawlerDb" on the first run, so you don't have to worry about setting up the database manually.

# Step 4: Verifying the Setup
**Checking the Database**: Once the application is running, it will automatically connect to the SQL Server and create the database. You can verify that the database has been created by logging into the SQL Server Management Studio and finding "CrawlerDb" in the list of databases.

# Step 5: You're All Set!
**You're Done!**: That's it! The application should now be running, and the database should be set up. You can now use the application as intended.

---

## Docker Setup Instructions
To get your application up and running using Docker, you'll be utilizing a few simple commands. Make sure you have Docker installed on your system. 

### Step 1: Navigate to the Project Directory
Open a terminal or command prompt and navigate to the directory where your project is located, specifically where the Dockerfile and docker-compose.yml files are situated. 

### Step 2: Building the Docker Image
Once in the project directory, execute the following command to build the Docker images as described in the docker-compose.yml file.

```sh
docker-compose build 
```

### Step 3: Running the Docker Containers
After building the images, you can start your application by running the following command. This will start the containers as described in the docker-compose.yml file:

```sh
docker-compose up
```

### Step 4: Stopping the Docker Containers
When you want to stop running the application, you can do so gracefully by using the following command in your terminal:

```sh
docker-compose down
```

### Step 5: Accessing the Application
Now, open your web browser and access the application by entering the localhost URL and port number where the application is running. 
Typing localhost will be enough!


## Important Note for Testing in a Local Environment

This application primarily relies on SQL Server and is not initially designed to be dockerized with SQLite. The inclusion of SQLite was facilitated to streamline the dockerization process and to avoid the complexities of authentication; it serves as a quick alternative for running the application in a Docker environment.


However, for a more comprehensive testing and better adherence to clean code principles, it is highly recommended to run the application in a local environment utilizing SQL Server. This would not only foster better code management but would also adhere closely to the application's initial design and architecture.

It's important to note that the dockerized version has known issues with the UI part, which might affect the overall user experience. These issues are not present when the application is set up locally.

Please follow the steps outlined above to configure and test the application in a local environment. 

Thank you for your attention to this matter.



