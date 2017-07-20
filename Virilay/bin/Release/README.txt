==VIRILAY==
=SETUP=
1. Run configurator
2. Fill out twitch settings
	-Chat Oauth should look something like this:
	    oauth:12018h30f13g083fg1
	-Setgame Oaauth should look something like this:
	    19471agka9eri0

=COMMANDS=
To execute a command, type '!<commandname> <param1> <param2> ... <paramn>'.
For example, 
	!twitch virifaux spyparty

void readme()
[HelpAttribute]
Generates a readme and opens it

string log(string text)
[HelpAttribute]
Writes text to a logfile. Returns the name of the file if we created a new file

void log()
[HelpAttribute]
Opens the log

void location()
[HelpAttribute]
Opens the folder where the exe is located

void compile(string filetitle)
[HelpAttribute]
Loads a sourcefile. All public methods will be accessible via this.Execute(). Requires that the classname and filename are the same. Aborts with no changes if there are conflicts in filename. Usage: compile('commands')

string help(string command)
[HelpAttribute,ViewerPermitted]
Shows what the function does

executereturn execute(string command, additionalparameters additionalparms)
[HelpAttribute]
Tries to issue the command. Returns true if successful.

void screenshot()
[HelpAttribute]
Creates a screenshot at the next available spot of Logs\screen<number>.png

void screenshot(string filename)
[HelpAttribute]
Creates a screenshot at <filename>.png

string twitch(string player1, string player2)
[HelpAttribute,ViewerPermitted]
Displays the multitwitch.tv link of both players

void taskexample()
[DebuggerStepThroughAttribute,AsyncStateMachineAttribute]

string topic()
[HelpAttribute]
Reports what the day's topic is

void topic(string topic)
[HelpAttribute]
Sets the topic

void eyetribe()

void obs()

void ts()

void spyparty()

void ping(string host, int32 num)

int32 random(int32 min, int32 max)
[ViewerPermitted,HelpAttribute]
Returns a number from [min,max], inclusive

int32 roll(string diceString)
[ViewerPermitted,HelpAttribute]
Rolls a die. eg. '1d6+2d3+3'

string longroll(string diceString)
[HelpAttribute,ViewerPermitted]
Rolls a die and shows the math. For example, LongRoll("3d6+2")

void qsplat(additionalparameters ap)
[AsyncStateMachineAttribute,DebuggerStepThroughAttribute,AdditionalParametersAttribute]

void splat(additionalparameters ap)
[AdditionalParametersAttribute,AsyncStateMachineAttribute,ViewerPermitted,DebuggerStepThroughAttribute]

void hi()

void settitle(string title)

void setgame(string game)

void adom()

void soup()

void do(string command)
[ViewerPermitted,AsyncStateMachineAttribute,DebuggerStepThroughAttribute]


=TYPES=
INTEGER
An integer number. Fractions not allowed

UINT
An unsigned integer number. Negative numbers, and fractions are not allowed. 0 is permitted.

DOUBLE, FLOAT, DECIMAL
A number that decimals, such as -3.3, 3, or 2

CHAR
A single character, such as 'a', '1', or 1.

STRING
Many characters, such as 3d6+2
If you want to include space, surround the words with ", such as:
	"This is a string"
