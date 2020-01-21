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

    saveStudy = (study, based) => {
        return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/study/save", {
          method: "post",
          headers: { 
            "Content-Type": "application/json", 
            "Authorization": "Bearer " + localStorage.getItem("SepesJWT"),
          },
          body: JSON.stringify([study, based])
        });
    }

    getSepesToken(azureAccountName, azureRawIdToken) {
      return fetch(process.env.REACT_APP_SEPES_BASE_URL+"/api/auth/token", {
        method: "post",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({"Usename": azureAccountName, "idToken": azureRawIdToken, "Expiration": "later"})
      });
    }
}
