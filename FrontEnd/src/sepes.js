export default class Sepes {
    newStudy = {
        studyName: "New study",
        userIds: [],
        datasetIds: []
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

    getData = async () => {
        return await fetch("https://localhost:5001/api/study/list").then(data => data.json());
    }

    setStudyName = (name) => {
        this.newStudy.studyName = name;
    }

    createStudy = () => {
        fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/study/create", {
          method: "post",
          headers: { 
            "Content-Type": "application/json", 
            "Authorization": "Bearer " + localStorage.getItem("SepesJWT"),
          },
          body: JSON.stringify(this.newStudy)
        });
      }
    
      addItemToStudy = (id, listName) => {
        this.newStudy[listName].push(id);
        console.log(this.newStudy[listName]);
      }
    
      removeItemFromStudy = (id, listName) => {
        this.newStudy[listName].splice(this.newStudy[listName].indexOf(id), 1);
        console.log(this.newStudy[listName]);
      }
}
