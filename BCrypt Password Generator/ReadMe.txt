BCrypt Password Generator - Designed and Created by Syed Z Abbas

The application performs three different functions
1. Generates a BCrypt Hash Password from a string
	a. Parameters are the unhashed password and the Length of the work factor
	b.  The work factor increases the time of hashing exponentially thus preventing dictionary attacks

2. Compares a given password with a BCrypt Hash
	a.The Parameters are the original Password and the hashed BCrypt sequence

3. Adds or searches the database for a user
	a. Add a new User
		i. Generate a new GUID
		ii. Fill in all the details and Click on "Convert to BCrypt"
		iii. Click on Add to DB

	b. Search for a User - Under Development
		i. Based on FirstName, LastName, GUID or Email 
		ii. Enter any one of the above and click on search, the rest of the fields populate themselves if a matching user is found.
