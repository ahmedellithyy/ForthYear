function [ squareCenter ] = QRFIP( img,L,rp )
    squareCenter=[];
    [h, ~] = size(img);
    for i = 1:h
        % [center counter color]
        freq = frequencies(img(i,:));
        [num, ~] = size(freq);
        
        for j = 3:num-3
            % black:white:black:white:black
            % 1:1:3:1:1 the rate of black and white pixels
            % error = 0.5;
            if     ceil((freq(j,2)/3)) < -(freq(j,2)/3)+freq(j-2,2) || ...
                   ceil((freq(j,2)/3)) < -(freq(j,2)/3)+freq(j+2,2) || ...
                   ceil((freq(j,2)/3)) < -(freq(j,2)/3)+freq(j-1,2) || ...
                   ceil((freq(j,2)/3)) < -(freq(j,2)/3)+freq(j+1,2) || ...
                   freq(j,3)==0 || L(i,freq(j,1))==0 || ...    %L(row index, column index from center in freq)
                   L(i,freq(j+2,1))==0 || L(i,freq(j-2,1))==0 || ...
                   L(i,freq(j+1,1))==1 || L(i,freq(j-1,1))==1
               
               continue;
            end
            
            % the two centers of small square and the line around the FIP
            % if it's less than 3 because of error but the perfect ration
            % is 0
            centersDifference = uint32(sum(abs(rp(L(i,freq(j+2,1))).Centroid-rp(  L(i, freq(j,1))   ).Centroid))); 
            
            % check the validity of the QR ration, 
            % if there's line around the square,
            % if the square isn't the same connected object
            if  centersDifference <= 3 && L(i,freq(j-2,1)) == L(i,freq(j+2,1)) && L(i,freq(j-2,1))~=L(i,freq(j,1))
              
                squareCenter = [squareCenter; L(i,freq(j,1))]; %center of the labeled connected object
                j = j+2;
            end
        end
    end

end

