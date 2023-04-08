# TASI
This is a project of mine, which is there, to make whitelisting storys unnessesary. Storys don't need to be whitelisted anymore, because dangerus functions will be disabled.

## At the current point of time
TASI is not finished just yet but will hopefully soon. Some proper tutorials will come when the syntax are more or less 100% done.


Here is some example code:

```
Name ExampleProgram; #Header - Set the name of the current namespace
Type Generic; #Header - Set namespace-type
Start { #Header - Set the start code for the project
    [Console.Clear] #Clear anything that's written in the console
    [Inf.DefVar:"Num","times"] #Create a var with the Num type called times
    [Console.WriteLine:"Enter the amount of times, you want to loop"] #Write to the console, so the user sees
    set times [Convert.ToNum:[Console.ReadLine], true]; #set times to a - from string to num - converted userinput and fail, if it can't be converted

    [Inf.DefVar:"String","printText"] #Create a var with the String type called printText
    [Console.WriteLine:"Enter the text you want to display"] #Write to the console, so the user sees
    set printText [Console.ReadLine]; #set printText to a userinput

    [Console.WriteLine:[ExampleProgram.DoWhileLoop:printText, times]] #Call the function and write the output to the console

    [Program.Pause:true] #Pause after program is done.
    return;
};


function string DoWhileLoop {string printText; num repeatLoop} #Define a function, that returns a string, and takes 2 variables as input
{
    [Inf.DefVar:"Num","i"] #Create a var with the Num type called i (it will be 0 as default)

    if (($repeatLoop) > 99999) #Check weather the loop amount is higher than 99999
    {
        return "Didn't succeed, because loop-amount was too large."; #If loop amount is too large, return with an error message.
    } 
    else 
    {
        if (($repeatLoop) < 1) #Check if the loop amount is smaller than 1
        {
            return "Didn't succeed, because loop-amount was too low."; #If loop amount is smaller than 1, return with an error message.
        };
    };

    while (($repeatLoop) > ($i)) #Repeat while repeatLoop is bigger than i
    {
        [Console.WriteLine:(($printText) + " | " + (($i) + 1) + " out of " + ($repeatLoop))] #Print loop to console.
        set i (($i) + 1);  #Set i to i + 1
    }; 
    return "Succeeded without problems (hopefully)."; #Everything should have been printed out successfully, so return with a message that says that.

};

```
You'll just have to put the code in a - by the interpreter accessable - file and enter the path.

