app.config([
   '$stateProvider', '$urlRouterProvider', '$locationProvider',
   function($stateProvider, $urlRouterProvider, $locationProvider) {

      $urlRouterProvider.otherwise('/');
      $locationProvider.html5Mode(true); // to remove # from URL_BASE

      // register parent and child states      
      for (var key in STATES) {
         
         var parentState = STATES[key];

         // register parent state
         if (parentState.hasOwnProperty('name')) 
            $stateProvider.state(parentState.name, parentState);
            
         // register all child states
         for (var subkey in parentState) {
            var childState = parentState[subkey];
            if (childState.hasOwnProperty('name'))
               $stateProvider.state(childState.name, childState);
         }
      }

   }
]);