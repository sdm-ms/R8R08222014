function ApplicationLoadHandler(sender, args) {
  // InitScript is a custom function 
  // registered from the User Control
  if(typeof InitScript == 'function')
    InitScript();
					
}

if( Sys && Sys.Application ){ 
  Sys.Application.add_load(ApplicationLoadHandler);
  Sys.Application.notifyScriptLoaded(); 
}