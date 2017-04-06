SEAMLESS LOOPING GIF
====================

- Download VideoLoops v2 / gif-jiggler by Microsoft<br>
`https://www.microsoft.com/en-us/download/details.aspx?id=52024`
- Download ffmpeg<br>
`https://ffmpeg.org/download.html`
- Download convert<br>
`https://joshmadison.com/convert-for-windows/`
- Take 5sec videos and convert it to one sec loop for example<br>
`VideoLoopCreate.exe -loopsecs 1 input.mp4`
- Get video frames and scale it to 640xX for a smaller size<br>
`mkdir frames
ffmpeg -i input_loop.mp4 -vf scale=640:-1:flags=lanczos,fps=10 frames/ffout%03d.png`
- Create gif that loops<br>
`convert -loop 0 frames/ffout*.png output.gif`
- Optimize it<br>
`http://gifgifs.com/optimizer/`

Tada you have now 1 second gif that loops seamlessly at about 600kb in size.


#### Useful commands
- Convert input video into smaller videos (3 seconds each for example)<br>
`ffmpeg -i tosplit.mp4 -map 0 -c copy -f segment -segment_time 3 -reset_timestamps 1 splitted/out%02d.mp4`
- Convert video to gif with ffmpeg only<br>
`ffmpeg -i video.mp4 -vf scale=500:-1 -t 10 -r 10 image.gif`
- Convert images to video<br>
`ffmpeg -framerate 24 -i shot%03d.png output.mp4`
`ffmpeg -framerate 24 -i shot%03d.png -pix_fmt yuv420p -vf "scale=trunc(iw/2)*2:trunc(ih/2)*2" output.mp4`
