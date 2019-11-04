import * as StudyService from "../studyService"


it("should set current study with one argument", () => {
    let study = {
        studyId: 1,
        studyName: "study",
        users: [1, 2],
        dataset: [1, 2, 3],
        pods: [{podId: 1, podNmae: "pod"}],
        archived: false
    }

    StudyService.setCurrentStudy(study)
    let result = StudyService.getCurrentStudy()

    expect(result).toEqual(study)

    StudyService.setCurrentStudy(null)
})

it("should set current study with multiple arguments", () => {
    let study = {
        studyId: 1,
        studyName: "study",
        users: [1, 2],
        dataset: [1, 2, 3],
        pods: [{podId: 1, podNmae: "pod"}],
        archived: false
    }

    StudyService.setCurrentStudyVars(study.studyId, study.studyName, study.users, study.dataset, study.pods, study.archived)
    let result = StudyService.getCurrentStudy()

    expect(result).toEqual(study)

    StudyService.setCurrentStudy(null)
})

it("should add unique rule to list", () => {
    let array = [{port: 1, ip: "1.1.1.1"}, {port: 2, ip: "1.1.1.1"}]
    let rule = {port: 3, ip: "1.1.1.1"}

    let result = StudyService.addRule(rule.port, rule.ip, array)
    let expected = [{port: 1, ip: "1.1.1.1"}, {port: 2, ip: "1.1.1.1"}, rule]
    
    expect(result).toEqual(expected)
})

it("should not add non-unique rule to list", () => {
    let array = [{port: 1, ip: "1.1.1.1"}, {port: 2, ip: "1.1.1.1"}]
    let rule = {port: 1, ip: "1.1.1.1"}

    let result = StudyService.addRule(rule.port, rule.ip, array)
    let expected = array
    
    expect(result).toEqual(expected)
})

it("should remove rule from list", () => {
    let list = [{port: 1, ip: "1.1.1.1"}, {port: 2, ip: "1.1.1.1"}]
    let rule = {port: 2, ip: "1.1.1.1"}

    let result = StudyService.removeRule(rule.port, rule.ip, list)
    let expected = [{port: 1, ip: "1.1.1.1"}]
    
    expect(result).toEqual(expected)
})

it("should add valid user to list", () => {
    let list = ["user1", "user2"]
    let result = StudyService.addUser("user3", list)

    expect(result).toEqual(["user1", "user2", "user3"])
})

it("should add valid user to list", () => {
    let list = ["user1", "user2"]
    let result = StudyService.removeUser("user2", list)

    expect(result).toEqual(["user1"])
})

it("should add dataset to list as strings", () => {
    let list = ["dataset1"]
    let result = StudyService.addDataset({DatasetId: 1, DatasetName: "dataset2"}, list)

    expect(result).toEqual(["dataset1", "dataset2"])
})

it("should remove dataset from list", () => {
    let list = ["dataset1", "dataset2"]
    let result = StudyService.removeDataset("dataset2", list)

    expect(result).toEqual(["dataset1"])
})
