app.config(function ($mdThemingProvider) {
   var customBlueMap = $mdThemingProvider.extendPalette('light-blue', {
      'contrastDefaultColor': 'light',
      'contrastDarkColors': ['50'],
      '50': 'ffffff'
   });
   $mdThemingProvider.definePalette('customBlue', customBlueMap);

   $mdThemingProvider.theme('default')
     .primaryPalette('customBlue', {
        'default': '800',
        'hue-1': '50'
     })
     .accentPalette('blue');

   $mdThemingProvider.theme('input', 'default')
      .primaryPalette('grey');

   $mdThemingProvider.theme('green').primaryPalette('green', {
      'default': 'A700',
      'hue-1': '50'
   }).accentPalette('green', {
         'default': 'A700',
         'hue-1': '50'
      });
});
/*
app.config(function ($mdThemingProvider) {
   var customBlueMap = $mdThemingProvider.extendPalette('light-blue', {
      'contrastDefaultColor': 'light',
      'contrastDarkColors': ['50'],
      '50': 'ffffff'
   });
   $mdThemingProvider.definePalette('customBlue', customBlueMap);
   $mdThemingProvider.theme('default')
      .primaryPalette('customBlue', {
         'default': '900',
         'hue-1': '50'
      })
      .accentPalette('pink');
   $mdThemingProvider.theme('input', 'default')
      .primaryPalette('grey');
});*/