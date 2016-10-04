angular.module('fileSystemApp').factory('fileSystemService', ['$http', function ($http) {
    const oneMb = 1048576;

    var fileSystemService = {
        getDirectoryContents: getDirectoryContents,
        postCountFilesInFolder: postCountFilesInFolder,
        getJobStatus: getJobStatus,
        getJobResult: getJobResult,
        cancelJob: cancelJob
    };
    
    return fileSystemService;


    function getDirectoryContents(path) {
        return $http.get("/api/filesystem/?" + encodeURIComponent(path));
    }

    function postCountFilesInFolder(countSizeFilter, path) {
        var countFilesSizeFilterDto = countSizeFilter.map(function(filter){
            return {
                fromBytes: filter.fromMb * oneMb,
                toBytes: filter.toMb * oneMb,
            }});
        return $http.post("/api/filesystem/count/?" + encodeURIComponent(path), countFilesSizeFilterDto);
    }

    function getJobStatus(jobId) {
        return $http.get("/api/filesystem/count/status/" + jobId);
    }

    function getJobResult(jobId) {
        return $http.get("/api/filesystem/count/" + jobId);
    }
    function cancelJob(jobId) {
        return $http.delete("/api/filesystem/count/" + jobId);
    }

}]);