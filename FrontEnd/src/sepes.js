export default class Sepes {
    newStudy = {
        studyName: "New study",
        userIds: [],
        datasetIds: [],
        archived: false,
    };

    getSupplierList = () => [{UserId: 1, UserName: "Ricardo Frame", UserEmail: "ricardo@sepes.com"}, 
                            {UserId: 2, UserName: "Gudrun Draugstad", UserEmail: "gudrun@sepes.com"}, 
                            {UserId: 3, UserName: "Arne Bjarne Bløffen", UserEmail: "arne@sepes.com"}, 
                            {UserId: 4, UserName: "Ivy Sadler", UserEmail: "ivy@sepes.com"}, 
                            {UserId: 5, UserName: "Lilly-May Hill", UserEmail: "lilly@sepes.com"}, 
                            {UserId: 6, UserName: "Gordon Macfarlane", UserEmail: "gordon@sepes.com"}, 
                            {UserId: 7, UserName: "Batman", UserEmail: "batman@batman.bat"},];

    getSponsorList = () => this.getSupplierList();

    getDatasetList = () => [{DatasetId: 1 , DatasetName: "Gudrun"}, 
                            {DatasetId: 2 , DatasetName: "Heidrun"}, 
                            {DatasetId: 3 , DatasetName: "Vale"}, 
                            {DatasetId: 4 , DatasetName: "Volve"}, 
                            {DatasetId: 5 , DatasetName: "Snøhvit"}, 
                            {DatasetId: 6 , DatasetName: "Troll"}, 
                            {DatasetId: 7 , DatasetName: "Sleipner"}, 
                            {DatasetId: 8 , DatasetName: "Gullfaks"}, 
                            {DatasetId: 9 , DatasetName: "Goliat"}, 
                            {DatasetId: 10 , DatasetName: "Kvitebjørn"}];
    GetDummyPodList = () => [{PodId: 0, PodName: "Testpod"},
                            {PodId: 1, PodName: "Goliat Data Refinement"},
                            {PodId: 2, PodName: "Snakeoil"},]

    getData = () => {
        return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/study/dataset");
    }

    getStudies(archived) {
        if (archived) {
            return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/study/archived");
        }
        return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/study/list");
    }

    initStudy = () => {
      this.newStudy = {
          studyName: "New study",
          userIds: [],
          datasetIds: [],
          archived: false,
      };
    }

    setStudyName = (name) => {
        this.newStudy.studyName = name;
    }

    createStudy = () => {
        console.log("Create study: "+this.newStudy.studyName+" - "+this.newStudy.datasetIds.length+" dataset");
        return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/pod/create", {
          method: "post",
          headers: { 
            "Content-Type": "application/json", 
            "Authorization": "Bearer " + localStorage.getItem("SepesJWT"),
          },
          body: JSON.stringify(this.newStudy)
        });
    }

    createPod = (studyID, podName) => {
        console.log(`Create pod: ${podName} - with study id ${studyID}`);
        return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/pod/create", {
          method: "post",
          headers: { 
            "Content-Type": "application/json", 
            "Authorization": "Bearer " + localStorage.getItem("SepesJWT"),
          },
          body: JSON.stringify({studyID, podName})
        });
    }

    getPods = (studyId) => {
        return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/pod/list/"+studyId);
    }
    
    addItemToStudy = (id, listName) => {
        this.newStudy[listName].push(id);
        console.log(this.newStudy[listName]);
    }
    
    removeItemFromStudy = (id, listName) => {
        this.newStudy[listName].splice(this.newStudy[listName].indexOf(id), 1);
        console.log(this.newStudy[listName]);
    }


    updateStudy(studyId, archived) {
      fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/study/update", {
        method: "post",
        headers: { 
          "Content-Type": "application/json", 
          "Authorization": "Bearer " + localStorage.getItem("SepesJWT"),
        },
        body: JSON.stringify({studyId, archived})
      });
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
