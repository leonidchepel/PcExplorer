angular.module('fileSystemApp').controller('fileSystemController', ['$window', 'fileSystemService', 'cookieService', function ($window, fileSystemService, cookieService) {
    const successfulStatus = 'Finished';
    fs = this;
    fs.lessFiles = 0;
    fs.middleFiles = 0;
    fs.bigFiles = 0;
    fs.files = [];
    fs.currentPath = "";
    fs.prevDirectory = "";
    fs.error = false;
    fs.getDirectoryContents = getDirectoryContents;
    fs.loadingDirectory = true;
    
    var jobId = null;
    var countSizeFilter = [
           {
               fromMb: 0,
               toMb: 10
           },
           {
               fromMb: 10,
               toMb: 50
           },
           {
               fromMb: 100,
               toMb: 0
           }
    ];


    activate();

    function activate() {
        var info = cookieService.getInfo();
        jobId = info.jobId;
        fs.currentPath = info.path;
        if (jobId == null) {
            fs.loadingCount = false;
        }
        getDirectoryContents(fs.currentPath);
        $window.onbeforeunload = saveCookies;
    }

    function saveCookies() {
        cookieService.setInfo(fs.currentPath, jobId);
    }

    function updateStatistics() {
        if (jobId !== null) {
            fileSystemService.getJobStatus(jobId).success(function successCallback(response) {
    
                if (response.status == successfulStatus) {
                    fileSystemService.getJobResult(jobId).success(function successCallback(data) {
                        
                        fs.lessFiles = data[0];
                        fs.middleFiles = data[1];
                        fs.bigFiles = data[2];
                        jobId = null;
                        fs.loadingCount = false;
                    });
                } else {
                    setTimeout(updateStatistics, 2000);
                }
                
            }).error(function errorCallback(error) {
                jobId = null;
                getJobIds();
            });

        }
    }


    function cancelJob(){
        if (jobId!==null) {
            fileSystemService.cancelJob(jobId);
            jobId = null;
        }
    }

    function getJobIds() {
        cancelJob();
       
            
        fileSystemService.postCountFilesInFolder(countSizeFilter, fs.currentPath).then(function successCallback(response) {
            jobId = response.data;
            fs.loadingCount = true;
            setTimeout(updateStatistics, 2000);
        });
       
    }

    function generatePrevDirectory() {
        var twoSlashParts = fs.currentPath.split('\\\\');
        if (twoSlashParts[0] == "" || twoSlashParts[1] == "") {
            fs.prevDirectory = "";
            return;
        }

        var oneSlashParts = twoSlashParts[1].split('\\');
        var prevPath = twoSlashParts[0] + '\\\\';
        for (var i = 0; i < oneSlashParts.length - 2; i++) {
            prevPath += oneSlashParts[i] + '\\';
        }
        fs.prevDirectory = prevPath;
    }

    function getDirectoryContents(path) {
        if (path == null) {
            path = "";
        }
        fs.loadingDirectory = true;
        fs.error = false;
        fileSystemService.getDirectoryContents(path).then(function successCallback(response) {
            
            fs.currentPath = path;
            if (path != "") {
                getJobIds();
            }
            var data = response.data;
           if (data.directories != null) {
                fs.files = data.directories.map(function (dir) {
                    return {
                        name: dir,
                        path: fs.currentPath + dir + '\\'
                    };
                });
            } 

           if (data.files != null) {
               var files = data.files.map(function (file) {
                   return {
                       name: file,
                       path: null
                   };

               });
               fs.files.push.apply(fs.files, files);
           }
           generatePrevDirectory();
           fs.loadingDirectory = false;
        }, function errorCallback(response) {
            fs.error = true;
            fs.loadingDirectory = false;
        });
    }

}]);