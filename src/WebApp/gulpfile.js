/// <binding Clean='clean' ProjectOpened='browserify' />
var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    project = require("./project.json");



var source = require('vinyl-source-stream');
var browserify = require('browserify');
var watchify = require('watchify');
var reactify = require('reactify');
var streamify = require('gulp-streamify');
var gutil = require('gulp-util');



var paths = {
    webroot: "./" + project.webroot + "/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

paths.entryPoint = paths.webroot + 'js/App.jsx';
paths.buildJS = paths.webroot + 'js/build.js';
paths.buildMinJS = paths.webroot + 'js/build.min.js';


gulp.task('browserify', function () {
    var bundler = browserify({
        entries: [paths.entryPoint], // Only need initial file, browserify finds the deps
        transform: [reactify], // We want to convert JSX to normal javascript
        debug: true, // Gives us sourcemapping
        cache: {}, packageCache: {}, fullPaths: true // Requirement of watchify
    });
    var watcher = watchify(bundler);

    return watcher
    .on('update', function () { // When any files update
        var updateStart = Date.now();
        console.log('Updating!');
        watcher.bundle()
    // Create new bundle that uses the cache for high performance
        .pipe(source(paths.buildJS))
    // This is where you add uglifying etc.
        .pipe(gulp.dest('.'));
        console.log('Updated!', (Date.now() - updateStart) + 'ms');
    })
    .bundle() // Create the initial bundle when starting the task
    .pipe(source(paths.buildJS))
    .pipe(gulp.dest('.'));
});

gulp.task('build', function () {
    browserify({
        entries: [paths.entryPoint],
        transform: [reactify],
    })
      .bundle()
      .pipe(source(paths.buildMinJS))
      .pipe(streamify(uglify(paths.buildMinJS)))
      .pipe(gulp.dest('.'))
      .on('error', gutil.log);
});



gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);
