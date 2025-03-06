module.exports = function (grunt) {
   grunt.initConfig({
      copy: {
         files: {
            src: ['**/*.js', '!**/node_modules/**', '!**/build/**', '!**/test/**'],
            dest: 'build',
            expand: true
         }
      }
   });
   grunt.loadNpmTasks('grunt-contrib-copy');
   grunt.registerTask('default', ['grunt-contrib-copy']);
};