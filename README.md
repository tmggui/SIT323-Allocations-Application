# Final Year Allocation Project <br />
###### Thomas Guilfoyle | SIT323 Cloud Application development | Deakin University

This is the final year project of the Cloud Computing major for the Bachelor of Information Technology <br />
at Deakin University. The class that this was conducted in was SIT323 Cloud Application Development with Dr Robert Dew. <br />
All work shown here is my own. <br />

The project was split into two parts, the core functionality of the project was built for Assignment 1, where students <br />
were given a zip folder with 8 custom txt files, 4 .taff files and 4 .cff files. We then had to create a program using C# <br />
and visual studio to validate each of the files so they satisfied certain criteria as described in the attached "Assessment Task 1.pdf". <br />
If each sequential file pair was valid, the program then had to process the allocations within the .taff file and produce <br />
and display timing and energy consumption data. If this data was within the limits of the .cff file the allocation was valid.

The second part of the project essentially built upon the assessment 1 program with the main difference being that instead of <br />
the .txt files being locally accessed by the program the files were hosted on Microsoft Azure servers and we were only given .cff files. <br />
The aim was to add C# code to the original program to access these files remotely, process them through the original validation code <br />
and then attempt to create our own .taff files with potential allocations to then be displayed. However, as this process could potentially <br />
take years to produce valid allocations using the resources of one computer, we had to add functionality to our project which could allow <br />
our program to connect to Amazon Web services and dynamically use and scale machines based on the workload of the program.

We finally had to create a short video to provide evidence of what we had done, this is attached below.


Link to evidence for marking video: https://u.pcloud.link/publink/show?code=XZjjpmXZV78iTgyx1u4bqUHouw5cK4ld5zGV
