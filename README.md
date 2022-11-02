# TwitterFollows
Code to fetch Twitter followers and friends

This app will fetch all the followers and/or people being followed by and account you identify by their user name and provide a CSV file containing the user's
ID, name and display name which you can use if you're trying to find folks you follow on a new platform or the like.  It requires you to provide your 
own Bearer Token string to use since Twitter rate limits apps.  It's easy enough to get your own token by creating a test app in the developer
site - this video walks you through the process and shows using the app:

https://www.lindborglabs.com/TwitterFollow/TwitterFollows.mp4

The app is kept as simple as possible using the .NET framework and raw HTTP calls to interact with Twitter's REST API.  Simply get your bearer token string, 
compile and go.

A Windows binary is available to folks that don't want to build the project themselves - the video above shows it in use.  You can download the ZIP file 
with the binaries you need from this link:

https://www.lindborglabs.com/TwitterFollow/TwitterFollows.zip

NOTE: This uses the free Twitter API "essentials" interface which is handy but they rate-limit it to 15 calls every 15 minutes.  If you have more than
1500 people you're following, it'll put up a dialog letting you know you've hit your rate limit and you can wait 15 minutes to continue and get the next 1500
and so on.  Not much to be done about this since I'm not paying for an enterprise app space for this!  Hopefully folks don't have huge numbers of people they're
following.

Hope folks find it helpful if they're moving off Twitter onto another platform.  

-Jeff
