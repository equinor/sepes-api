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

export function addUser(user, array) {
    if (validateUser(user)) {
        return [...array, user]
    }

    return array
}

export function removeUser(user, array) {
    let index = array.indexOf(user)
    let newArray = [...array]
    newArray.splice(index, 1)

    return newArray
}

// takes dataset {DatasetId: number, DatasetName: string} and returns an array of dataset names 
export function addDataset(dataset, array) {
    return [...array, dataset.DatasetName]
}

// takes datasetName as string and returns a new array
export function removeDataset(datasetName, array) {
    let newArray = [...array]
    newArray.splice(newArray.indexOf(datasetName), 1)

    return newArray
}


function validateUser(user) {
    if (typeof user === "string") {
        return true
    }
    return false
}


export function addRule(port, ip, array) {
    if (findRuleIndex(array, Number(port), ip) === -1) {
        return [...array, {port: Number(port), ip}]
    }

    return array
}

export function removeRule(port, ip, array) {
    let newArray = [...array];
    newArray.splice(findRuleIndex(newArray, port, ip), 1);

    return newArray
}


function findRuleIndex(array, port, ip) {
    return array.findIndex((item) => (item.port === port && item.ip === ip))
}
