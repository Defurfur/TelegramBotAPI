# Review notes

## General notes

1. What's up with environments? 
	Release is used for publishing. 
	Debug is used for development. 
	
	What's up with environment specific info? 

	1.1 DB - create production db or make a production-specific user for existing one. Up to you
	1.2 Make distinct bot environments to separate working prod version of the debug one. 
	

2. Check styles of the json files. Properties should be sorted. 

3. Try to avoid using inline local functions. They make code simple only on the first glance. 
Any further changes will make things more complex without a reason. 

4. Write summaries. It makes life much easier for everyone.

5. Complexity. Overall complexity of the solution is way to high. 
Injections are very coupled, some classes are tight to much together. 
Overall solution requires refactoring of some bad places (look into 'review' search entries)


## Additional thoughts

1. Use Native AoT - bot always will work on Windows, hence it's not required to include 
framework itself and additional platforms build data. AoT build is smaller and works faster.

2. Check some complex places. Maybe some pattern usages can be simplyfied.

## Performance

Code metrics should be checked in some ways. 

1. Current scheduled functions timing metrics should be obtained and researched. 
Result should be:
- Average scheduler execution time is xxx seconds. 

2. Api performance metrics should be examined 
Result should be:
- Memory allocations are xxx mb per user/ per task
- Memory inefficient parts of code are - xxxxx.
- Ways of reducing allocations and releasing memory and cpu-time are - xxxx. 

3. Load testing is required - real application performance boundaries should be found

Result should be:
- Application working on current machine can serve xx telegram requests per seconds
- Application working on a current machine can serve x groups update tasks without 
performance impact on the main API.

According to this metrics the decision should be made - is application runtime optimal, and if it is not, 
where and how can it be updated. 
