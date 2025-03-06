declare namespace NodeJS {
   interface Global { // TODO global pollution it must be removed after TS rewrite
      ErrorResponse: any,
      SuccessResponse: any,
      LOG: any,
      MSError: any,
      MSInternalServerError: any,
      MSSevereError: any,
      commonConstants: any,
      managementServerUrls: any,
      watcherServersUrls: any,
      languageTable: any,
      plugins: any,
      pluginConstants: any,
      myRequestValidation: any,
      include: any,
      utils: CommonUtils;
      getGuartinelHome: any;
   }

   interface String2 {
      tryStringify(text);
      isNullOrEmpty(text);
      traceIfNull(key);
      getProcessInfo():string;
      generateUUID():string;
   }
   interface Object2 {
      ensureAsObject(data);
      isObject(data);
      isNull(data);
      isNullOrEmpty(data);
      sanitizePropertyValuesByKey(list, obj);
   }

   interface CommonUtils {
      string: String2;
      object: Object2;
   }
}
