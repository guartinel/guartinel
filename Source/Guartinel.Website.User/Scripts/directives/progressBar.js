app.directive('progressbar', function () {
   var result = {
      restrict: 'E',
      link: function (scope, element, attrs) {

         // js doesn't like ',' in numbers
         attrs.value = attrs.value.replace(",", ".");
         if (attrs.value == null || attrs.value == undefined)
            attrs.value = 0;

         var options = {
            data: Math.round(attrs.value),
            numMin: 0,
            numMax: 100,

            labelVisibility: 'hidden',
            labelColor: 'black',
            
            horBarHeight: 22,
            forceAnim: true,
            backColor: 'rgb(212, 212, 212)',
            colorRange: true,
            colorRangeLimits: {
                optimal: '0-' + (attrs.alert*0.75) + '-green',
                high: (attrs.alert*0.75) +'-'+ attrs.alert + '-rgb(244, 144, 40)',
                critical: attrs.alert +'-100'
            },

            milestones: {
               1: {
                  mlPos: attrs.alert,
                  mlId: false,
                  mlClass: 'bi-custom',
                  mlDim: '150%',
                  mlLabel: attrs.alert + '%',
                  mlLabelVis: 'visible',
                  mlHoverRange: 15,
                  mlLineWidth: 2,
                  mlLineHeight: 15,
                  //mlLinezindex: 1,
               }
            }
         }

         angular.element(element).barIndicator(options);
      }
   }

    return result;
});