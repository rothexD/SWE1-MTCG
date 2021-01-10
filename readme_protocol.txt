Author: if19b205
designs:
	Route based system
	small classes, majority of classes are helperclasses.
	Game lives in Routes and Database, no caching of players / decks.... big scale
	added /help and /hello route
		/help
			responds with Json array of objects, listing possible commands and params/payload
	each Route is independed of all others.
	Pitfalls:
		Coded to much early on, not a lot of code was reuseable once i saw the curl script.
		Only one DatabaseReader allowed per DatabaseConnection, need to close before next command.
Unittets:
	Chosen to ensure DamageCalculation of Cards works as intended even if new cards are added, old cards should behave as was.
TimeTrack
	Rougly 12-15h in the beginning, out of which alot of code was not reuseable.. effective 7h
	Routes Total 18.... roughly 16h
	Bugfixes 3h
	Total: 36h
	
