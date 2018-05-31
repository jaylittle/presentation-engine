# Presentation Engine 5.0

This repo is a journal of my quest to port the Presentation Engine code base to ASP.NET Core.  Current Status: Success!

### 1. What is the timeline for this?

I have officially completed beta testing the current version. Feel free to clone, build and install to your hearts content!

### 2. Would you recommend using this right now?

It screams like a bat out of hell and is relatively secure. If you don't like those things, don't use it now.

### 3. What the hell is this project anyway?

My version of a basic blog.  More info is available here:  http://jaylittle.com/article/view/presentation-engine

### 4. Your previous versions didn't have a license? What gives?

I've gotten old enough to give a shit. FOSS has done a lot of good for me over the years and its well past time I start returning the favor.

### 5. FOSS? This is .NET you silly man!

Yeah this port is being developed 100% on Linux.  The goal is for it to be able to work in a cross platform context, but I will be primarily developing it using tools available to me on Arch Linux.

### 6. What tools?

VS Code is my primary development tool. Although I have a Rider license, I just haven't been able to get that into it.  A lot of this has to do with the fact that it lagged VS Code in support .NET Core 2.0 which I transitioned to pretty aggressively (e.g. days after it was released).  Bottom Line: For me the jury is still out on Rider, but I love VS Code.  Beyond that I've continued to use Gimp for the graphical work and I have made extensive use of DataGrip for working with sqlite databases.

### 7. What do I need to have installed to run this?

Node.js and the .NET Core 2.1 runtime.  We utilize npm to pull down node packages we make use of which includes babel and webpack among others.  The .NET Core 2.1 runtime is a no brainer of course.  If you want to hack in the codebase, you'll want to also have the .NET Core 2.1 SDK installed.
