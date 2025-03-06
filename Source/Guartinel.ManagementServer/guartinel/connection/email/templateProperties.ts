export namespace TemplateProperties {
 export  enum Visibility {
      visible = "visible",
      hidden = "hidden",
   }

   export  enum StatusColor {
      alerting = "#E84849",
      recovered = "#6AC259",
      partiallyRecovered = "#e8bf47"
   }

   export enum Status {
      alerting = "alerting",
      recovered = "recovered",
      partiallyRecovered = "partiallyRecovered"
   }

   export enum StatusCaption {
      alerting = "Alerting!",
      recovered = "Recovered!",
      partiallyRecovered = "Partially recovered!"
   }
}