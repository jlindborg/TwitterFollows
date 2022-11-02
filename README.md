# TwitterFollows
Code to fetch Twitter followers and friends

This app will fetch all the followers and/or people being followed by and account you identify by their user name and provide a CSV file containing the user's
ID, name and display name which you can use if you're trying to find folks you follow on a new platform or the like.  It requires you to provide your 
own Bearer Token string to use since Twitter rate limits apps.  It's easy enough to get your own token by creating a test app in the developer
site - this video walks you through the process and shows using the app:


The app is kept as simple as possible using the .NET framework and raw HTTP calls to interact with Twitter's REST API.  Simply get your bearer token string, 
copy it into the variable at the top of the main form code, compile and go.
