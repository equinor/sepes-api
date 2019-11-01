var studies = []

export var currentStudy = {
    studyId: null,
    studyName: "",
    users: [],
    dataset: [],
    pods: [],
    archived: false
}


export function getCurrentStudy() {
    return currentStudy
}

export function setCurrentStudy(study) {
    currentStudy = study
}

export function setCurrentStudyVars(studyId = currentStudy.studyId, 
                                    studyName = currentStudy.studyName, 
                                    users = currentStudy.users, 
                                    dataset = currentStudy.dataset, 
                                    pods = currentStudy.pods,
                                    archived = currentStudy.archived) {
    currentStudy = {studyId, studyName, users, dataset, pods, archived}
}

export function addUser(user, array = []) {
    if (validateUser(user)) {
        return [...array, user]
    }

    return array
}

export function removeUser(user, array = []) {
    let index = array.indexOf(user)
    let newArray = [...array]
    newArray.splice(index, 1)

    return newArray
}

export function addDataset(dataset, array = []) {
    return [...array, dataset.DatasetName]
}

export function removeDataset(dataset, array = []) {
    let newArray = [...array]
    newArray.splice(newArray.indexOf(dataset), 1)

    return newArray
}




function validateUser(user) {
    if (typeof user === "string") {
        return true
    }
    else {
        return true
    }
}
