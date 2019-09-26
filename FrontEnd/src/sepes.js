export default class Sepes {
    newStudy = {
        studyName: "New study",
        userIds: [],
        datasetIds: []
    }

    getSupplierList = () => [{UserId: 1, UserName: "Ricardo Frame", UserEmail: "ricardo@sepes.com"}, 
                            {UserId: 2, UserName: "Gudrun Draugstad", UserEmail: "gudrun@sepes.com"}, 
                            {UserId: 3, UserName: "Arne Bjarne Bløffen", UserEmail: "arne@sepes.com"}, 
                            {UserId: 4, UserName: "Ivy Sadler", UserEmail: "ivy@sepes.com"}, 
                            {UserId: 5, UserName: "Lilly-May Hill", UserEmail: "lilly@sepes.com"}, 
                            {UserId: 6, UserName: "Gordon Macfarlane", UserEmail: "gordon@sepes.com"}, 
                            {UserId: 7, UserName: "Batman", UserEmail: "batman@batman.bat"},]

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
        switch(listName) {
          case "datasetIds": this.newStudy.datasetIds.push(id); break;
          case "userIds": this.newStudy.userIds.push(id); break;
          default: break;
        }
      }
    
      removeItemFromStudy = (id, listName) => {
        switch(listName) {
          case "datasetIds": remove(this.newStudy.datasetIds); break;
          case "userIds": remove(this.newStudy.userIds); break;
          default: break;
        }
        
        function remove(array) {
          array.splice(array.indexOf(id), 1);
        }
      }
}
