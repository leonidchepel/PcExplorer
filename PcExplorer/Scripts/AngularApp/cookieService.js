angular.module('fileSystemApp').factory('cookieService', ['$cookies', function ($cookies) {
    const pathCookie = 'fs_PATH';
    const jobIdCookie = 'fs_JOBID';

    var cookieService = {
        getInfo: getInfo,
        setInfo: setInfo
    };

    return cookieService;

    function getInfo() {
        return {
            path: $cookies.get(pathCookie),
            jobId: $cookies.get(jobIdCookie)
        }
    }

    function setInfo(path, jobId) {
        $cookies.remove(pathCookie);
        $cookies.remove(jobIdCookie);
        $cookies.put(pathCookie, path);
        $cookies.put(jobIdCookie, jobId);
    }
   
}]);