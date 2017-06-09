REM This script requires the following tool to be installed and working in the same directory:
REM https://github.com/ErikEJ/SqlCeCmd

REM It also requires that a copy of your current PEngine database be in the current directory 
REM and be named pengine.sdf. NOTE: This upgrade path is NOT compatible with pre 4.0 versions.

SqlCeCmd40.exe -d "Data Source=.\pengine.sdf" -q "SELECT * FROM Article" -x -o Article.xml
SqlCeCmd40.exe -d "Data Source=.\pengine.sdf" -q "SELECT * FROM ArticleSection" -x -o ArticleSection.xml
SqlCeCmd40.exe -d "Data Source=.\pengine.sdf" -q "SELECT * FROM Post" -x -o Post.xml
SqlCeCmd40.exe -d "Data Source=.\pengine.sdf" -q "SELECT * FROM ForumUser" -x -o ForumUser.xml
SqlCeCmd40.exe -d "Data Source=.\pengine.sdf" -q "SELECT * FROM Forum" -x -o Forum.xml
SqlCeCmd40.exe -d "Data Source=.\pengine.sdf" -q "SELECT * FROM ForumThread" -x -o ForumThread.xml
SqlCeCmd40.exe -d "Data Source=.\pengine.sdf" -q "SELECT * FROM ForumThreadPost" -x -o ForumThreadPost.xml
SqlCeCmd40.exe -d "Data Source=.\pengine.sdf" -q "SELECT * FROM ResumeObjective" -x -o ResumeObjective.xml
SqlCeCmd40.exe -d "Data Source=.\pengine.sdf" -q "SELECT * FROM ResumePersonal" -x -o ResumePersonal.xml
SqlCeCmd40.exe -d "Data Source=.\pengine.sdf" -q "SELECT * FROM ResumeSkill" -x -o ResumeSkill.xml
SqlCeCmd40.exe -d "Data Source=.\pengine.sdf" -q "SELECT * FROM ResumeEducation" -x -o ResumeEducation.xml
SqlCeCmd40.exe -d "Data Source=.\pengine.sdf" -q "SELECT * FROM ResumeWorkHistory" -x -o ResumeWorkHistory.xml