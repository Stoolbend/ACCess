# ACCess

A simple client *serverList.json* manager for Assetto Corsa Competizione.

---

ACCess allows you to easily & quickly change the contents of the newly introduced **serverList.json** file, 
letting you connect directly to your favourite online racing communities

## Screenshot
![Basic program usage](https://raw.github.com/Stoolbend/ACCess/main/Screenshots/1.gif)

## Usage
- When the tool is opened, it will check for the existence of *serverList.json* inside the default ACC configuration folder,
 `%userprofile%\Documents\Assetto Corsa Competizione\Config`. If a file is found then it will display the currently saved server.
- Enter the new IP address you wish to use into the **Currently selected server** box in the top left, then click the green **Update** button in the bottom left.
- If you wish to remove the *serverList.json* file (eg: no longer wish to connect directly to an ACC server), click the red **Clear** button in the bottom left.

#### Favourites

Favourite servers can be added using the right side of the tool.

- Enter the IP address of the server you wish to save as well as an optional description, then click **Add** in the bottom right.
- To delete a favourite, select it using the list on the right and then select **Delete Selected** in the top right.
- When you select a server on the right, it will overwrite the IP address in the **Currently selected server** box on the left.
  **Remember to click Update after selecting a server! 😁**

### Config directory

The *Config directory* option on the left is provided for users who have non-standard system configurations, 
where the ACC folder may not be in the usual location assumed above. The config directory should not need to
be changed, but the option is provided *just in case* 😅.