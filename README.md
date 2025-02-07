﻿# raft-Cortlynd101
## Election Tests
- [X] 1. When a leader is active it sends a heart beat within 50ms.
- [X] 2. When a node receives an AppendEntries from another node, then first node remembers that other node is the current leader.
- [X] 3. When a new node is initialized, it should be in follower state.
- [X] 4. When a follower doesn't get a message for 300ms then it starts an election.
- [X] 5. When the election time is reset, it is a random value between 150 and 300ms.
    - 1. between
    - 2. random: call n times and make sure that there are some that are different (other properties of the distribution if you like)
- [X] 6. When a new election begins, the term is incremented by 1.
    - 1. Create a new node, store id in variable.
    - 2. wait 300 ms
    - 3. reread term (?)
    - 4. assert after is greater (by at least 1)
- [X] 7. When a follower does get an AppendEntries message, it resets the election timer. (i.e. it doesn't start an election even after more than 300ms)
- [X] 8. Given an election begins, when the candidate gets a majority of votes, it becomes a leader. (think of the easy case; can use two tests for single and multi-node clusters)
- [X] 9. Given a candidate receives a majority of votes while waiting for unresponsive node, it still becomes a leader.
- [X] 10. A follower that has not voted and is in an earlier term responds to a RequestForVoteRPC with yes. (the reply will be a separate RPC)
- [X] 11. Given a candidate server that just became a candidate, it votes for itself.
- [X] 12. Given a candidate, when it receives an AppendEntries message from a node with a later term, then candidate loses and becomes a follower.
- [X] 13. Given a candidate, when it receives an AppendEntries message from a node with an equal term, then candidate loses and becomes a follower.
- [X] 14. If a node receives a second request for vote for the same term, it should respond no. (again, separate RPC for response)
- [X] 15. If a node receives a second request for vote for a future term, it should vote for that node.
- [X] 16. Given a candidate, when an election timer expires inside of an election, a new election is started.
- [X] 17. When a follower node receives an AppendEntries request, it sends a response.
- [X] 18. Given a candidate receives an AppendEntries from a previous term, then rejects.
- [X] 19. When a candidate wins an election, it immediately sends a heart beat.

## Log Replication Tests
- [X] 1. When a leader receives a client command the leader sends the log entry in the next appendentries RPC to all nodes
- [X] 2. When a leader receives a command from the client, it is appended to its log
- [X] 3. When a node is new, its log is empty
- [X] 4. When a leader wins an election, it initializes the nextIndex for each follower to the index just after the last one it its log
- [X] 5. Leaders maintain an "nextIndex" for each follower that is the index of the next log entry the leader will send to that follower
- [X] 6. Highest committed index from the leader is included in AppendEntries RPC's
- [X] 7. When a follower learns that a log entry is committed, it applies the entry to its local state machine
- [X] 8. When the leader has received a majority confirmation of a log, it commits it
- [X] 9. The leader commits logs by incrementing its committed log index
- [X] 10. Given a follower receives an appendentries with log(s) it will add those entries to its personal log
- [X] 11. A followers response to an appendentries includes the followers term number and log entry index
- [X] 12. When a leader receives a majority responses from the clients after a log replication heartbeat, the leader sends a confirmation response to the client
- [X] 13. Given a leader node, when a log is committed, it applies it to its internal state machine
- [X] 14. When a follower receives a valid heartbeat, it increases its commitIndex to match the commit index of the heartbeat
    - Reject the heartbeat if the previous log index / term number does not match your log
- [X] 15. When sending an AppendEntries RPC, the leader includes the index and term of the entry in its log that immediately precedes the new entries
    - If the follower does not find an entry in its log with the same index and term, then it refuses the new entries 
        - Term must be same or newer
        - If index is greater, it will be decreased by leader
        - If index is less, we delete what we have
    - If a follower rejects the AppendEntries RPC, the leader decrements nextIndex and retries the AppendEntries RPC
- [X] 16. When a leader sends a heartbeat with a log, but does not receive responses from a majority of nodes, the entry is uncommitted
- [X] 17. If a leader does not response from a follower, the leader continues to send the log entries in subsequent heartbeats  
- [X] 18. If a leader cannot commit an entry, it does not send a response to the client
- [X] 19. If a node receives an appendentries with a logs that are too far in the future from your local state, you should reject the appendentries
- [X] 20. If a node receives and appendentries with a term and index that do not match, you will reject the appendentry until you find a matching log 
