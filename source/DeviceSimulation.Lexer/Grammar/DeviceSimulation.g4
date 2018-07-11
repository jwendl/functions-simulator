grammar DeviceSimulation;

prog
	: (line? EOL) + line?
	;

line
	: cmd + comment?
	| comment
	;

cmd
	: start
	| chance
	| vary
	| random
	;

quotedstring
   : '[' (quotedstring | ~ ']')* ']'
   ;

name
   : STRING
   ;

value
   : STRINGLITERAL
   | expression
   | deref
   ;

signExpression
   : (('+' | '-'))* (number | deref)
   ;

multiplyingExpression
   : signExpression (('*' | '/') signExpression)*
   ;

expression
   : multiplyingExpression (('+' | '-') multiplyingExpression)*
   ;

number
   : NUMBER
   ;

comment
   : COMMENT
   ;

deref
   : ':' name
   ;

start
	: 'start' expression
	;

chance
	: 'chance' expression
	;

vary
	: 'vary' expression
	;

random
	: 'random'
	;

STRINGLITERAL
   : '"' STRING
   ;


STRING
   : [a-zA-Z] [a-zA-Z0-9_]*
   ;


NUMBER
   : [0-9] +
   ;


COMMENT
   : ';' ~ [\r\n]*
   ;


EOL
   : '\r'? '\n'
   ;


WS
   : [ \t\r\n] -> skip
   ;