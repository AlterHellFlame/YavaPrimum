import { DateTime } from "luxon";
import { Candidate } from "./Candidate.interface";

export interface Tasks
{
    candidate: Candidate,
    status: boolean
    dateTime: DateTime,
    taskType: string
}

export interface TasksRequest
{
    candidate: Candidate,
    dateTime: string,
}
