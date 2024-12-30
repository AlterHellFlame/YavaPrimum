import { DateTime } from "luxon";
import { Candidate, createDefaultCandidate } from "./Candidate.interface";

export interface Tasks
{
    taskResponseId: string,
    candidate: Candidate,
    status: boolean
    dateTime: DateTime,
    taskType: string
}

export function createDefaultTasks(): Tasks {
    return {
      taskResponseId: 'default-id',
      candidate: createDefaultCandidate(),
      status: false,
      dateTime: DateTime.now(),
      taskType: 'default-task-type'
    };
  }

export interface TasksRequest
{
    candidate: Candidate,
    dateTime: string,
}

