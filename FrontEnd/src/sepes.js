export default class Sepes {

    getData = () => {
        return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/study/dataset");
    }

    getStudies(archived) {
        if (archived) {
            return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/study/archived");
        }
        return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/study/list");
    }

    createStudy = (study, based) => {
        console.log("Save study: "+study.studyName);
        console.log([study, based]);
        return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/study/save", {
          method: "post",
          headers: { 
            "Content-Type": "application/json", 
            "Authorization": "Bearer " + localStorage.getItem("SepesJWT"),
          },
          body: JSON.stringify([study, based])
        });
    }

    createPod = (studyID, podName) => {
        /*console.log(`Create pod: ${podName} - with study id ${studyID}`);
        return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/pod/create", {
          method: "post",
          headers: { 
            "Content-Type": "application/json", 
            "Authorization": "Bearer " + localStorage.getItem("SepesJWT"),
          },
          body: JSON.stringify({studyID, podName})
        });*/
    }

    getPods = (studyId) => {
        return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/pod/list/"+studyId);
    }

    updateStudy(studyId, archived) {
      /*fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/study/update", {
        method: "post",
        headers: { 
          "Content-Type": "application/json", 
          "Authorization": "Bearer " + localStorage.getItem("SepesJWT"),
        },
        body: JSON.stringify({studyId, archived})
      });*/
    }

    getSepesToken(azureAccountName, azureRawIdToken) {
      console.log("getSepesToken()");
      return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/auth/token", {
        method: "post",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({"Usename": azureAccountName, "idToken": azureRawIdToken, "Expiration": "later"})
      });
    }
}
