== Hello UpThere! ==
This project hasn't been updated to use Twitch API v5 (this currently uses v2!)
As such, it doesn't actually connect to the Twitch API. Feel free to play with
the following features:
   -Any commands listed in the COMMANDS reference below, such as
       !splat
       !ping google.com 3
       !roll 3d6
       !asynchronous
       !help roll
   -Write your own commands by creating cs files in the
    UserScripts/Autoload directory.
   -Open notepad (or other application), and input the following commands
       !play notepad
       !type "hello world" 
       !rightclick
 
==CREATING COMMANDS==
Any command tagged as 'viewerpermitted' can be run by viewers
For Virilay, to allow viewers access to this command
     void Roll(string dieroll)
Add the following code
     [ViewerPermitted]
     void Roll(string dieroll)
If you wanted to add help text when users type !help, use the
help attribute.

Use Console.WriteLine or return to report text
If you want to use an external framework, add the dlls to
UserScripts/dll. Remember to also create a text file!

==RULES REFERENCE==
=VIRI.LIEUTENANTS.EXECUTOR=
void AddLieutenant(object uses)

string Help(string command)
Shows the help text. eg.Help('splat'), Help('interactive.splat')
[HelpAttribute]
[ViewerPermittedAttribute]

boolean Execute(string inputstring, additionalparameters additionalparms, object[]& refvals)
Tries to issue the command. Returns true if successful.
[HelpAttribute]

string Classes()

[HelpAttribute]
[ViewerPermittedAttribute]

void Readme()
Generates a readme and opens it
[HelpAttribute]

string NextFile(string prefix, string filetype)
Generates the next unique filename. For example, NextFile("Logs\log",".log") might generate 'Logs\log3.log'
[HelpAttribute]

string Log(string text, string origin)
Writes text to a logfile. Returns the name of the file if we created a new file
[HelpAttribute]
[ViewerPermittedAttribute]

string Log(string text, additionalparameters ap)
Writes text to a logfile. Returns the name of the file if we created a new file
[HelpAttribute]
[ViewerPermittedAttribute]
[AdditionalParametersAttribute]

void Log()
Opens the log
[HelpAttribute]

void Location()
Opens the working directory, where the exe is stored
[HelpAttribute]

boolean Compile(string filetitle)
Loads a sourcefile. Returns true if successfully compiled. Otherwise, reports the reason in 'reason'
[HelpAttribute]

void Dispose()

void Allow(string command)
Prevents plebians from using the function
[HelpAttribute]

boolean CanConvert(string value, type type)

=COMMANDS=
void Screenshot()
Creates a screenshot at the next available spot of Logs\screen<number>.png
[HelpAttribute]

void Screenshot(string filename)
Creates a screenshot at <filename>.png
[HelpAttribute]

string Twitch(string player1, string player2)
Displays the multitwitch.tv link of both players
[HelpAttribute]
[ViewerPermittedAttribute]

void Obs()

void TS()

void SpyParty()

void Ping(string host, int32 num)

void Asynchronous()

int32 Random(int32 min, int32 max)
Returns a number from [min,max], inclusive
[ViewerPermittedAttribute]
[HelpAttribute]

int32 Roll(string diceString)
Rolls a die. eg. '1d6+2d3+3'
[HelpAttribute]
[ViewerPermittedAttribute]

string LongRoll(string diceString)
Rolls a die and shows the math. For example, LongRoll("3d6+2")
[HelpAttribute]
[ViewerPermittedAttribute]

=INTERACTIVE=
void BB()
[ViewerPermittedAttribute]

void BB(int32 timestart, int32 timerange)
[DebuggerStepThroughAttribute]
[ViewerPermittedAttribute]
[AsyncStateMachineAttribute]

void QSplat(additionalparameters ap)
[AsyncStateMachineAttribute]
[AdditionalParametersAttribute]
[DebuggerStepThroughAttribute]

void Splat(additionalparameters ap)
Shows viewer <3<3<3.
[ViewerPermittedAttribute]
[HelpAttribute]
[AdditionalParametersAttribute]

void Heart(additionalparameters ap)
Shows viewer <3<3<3.
[HelpAttribute]
[ViewerPermittedAttribute]
[AdditionalParametersAttribute]

void Shame(additionalparameters ap)
[ViewerPermittedAttribute]
[AdditionalParametersAttribute]

=TWITCHCOMMANDS=
void SetTitle(string title)
[ViewerPermittedAttribute]

void SetGame(string game)
[ViewerPermittedAttribute]

=TWITCHPLAYS=
void Play(string game)

void Play(string game, int32 index)
[AsyncStateMachineAttribute]
[DebuggerStepThroughAttribute]

void RightClick()
[DebuggerStepThroughAttribute]
[AsyncStateMachineAttribute]

void Type(string command)
[ViewerPermittedAttribute]

intptr FindWindow(string lpClassName, string lpWindowName)
[DllImportAttribute]
[PreserveSigAttribute]

boolean SetForegroundWindow(intptr hWnd)
[DllImportAttribute]
[PreserveSigAttribute]


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
