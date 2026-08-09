Go through the codebase and update the README.md file, also include the tagline of 'Built by Gamers, For Gamers'
----------
create the README file, in such a way that it understandable for every users and gamers, not entirely in the developer perspective
------------------------------------
Check out the main branch and update the changes to that branch and return back to the master branch

I'll help you check out the main branch, update changes there, and return to master. Let me start by checking the current git status.

git status
git checkout main
git merge master
git checkout master

### 

---

I want you to create a Design file `/.github/design.md`, detailly explain the design and architecture of this project.
Things to remember,
- Use mermaid code for Architecture diagram
- Highlight / differentiate the important component in the mermaid code (Don't make a cookie cutter diagram)
- Follow the DRY priciple
- It should be tuned for GitHub copilot 


- Don't repeat the same thing again and again.
- Don't sacrifice the content

---

-The above given structure is only for reference purpose, don't implement as it is (use it as a reference) and stick to our core principles

---


create @mai from design.md


I want to come up with list of tech stack for this project and suggest the optimal one (for e.g. Database - Sqlite or json storage)








build a ci verfication of compilation for main branch

?



WPF means Windows Presentation Foundation. It is Microsoft’s Windows desktop UI framework for C#/.NET. The interface is normally defined using XAML, while C# handles UI behavior. In this project, WPF would power only the Settings window—not the Rust optimizer engine.



Runner starts.
Open SQLite once and keep the connection available.
Query app_state.active_profile_id.
Load that profile, its macros and related settings in one transaction/query.
Keep the resulting profile in memory.
Use the in-memory copy during optimization.