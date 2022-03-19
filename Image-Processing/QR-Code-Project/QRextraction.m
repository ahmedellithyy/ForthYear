function [  ] = QRextraction( )
    Rimg = imread('1.jpg');
    figure,imshow(Rimg);
    img = rgb2gray(Rimg);
    
    %image preparing
    img = imsharpen(img,'Radius',10);  
    img = ~imbinarize(img); % will be used in hits
    se = strel('square',5);
    img = imdilate(img,se);
    figure,imshow(img);
    
    
    %labeling the image to detect the 8-components
    [L, ~] = bwlabel(img);
    rp = regionprops(L,'all');
   
    squareCenter = QRFIP(img,L,rp);
    squareCenter = [squareCenter ; QRFIP(imrotate(img, -45),imrotate(L, -45), rp)];    %the extra line 
    squareCenter = [unique(squareCenter) histc(squareCenter, unique(squareCenter))];
    disp(squareCenter)
    [row, ~]=size(squareCenter);
    
    %------------------------------------------------
    flag = zeros(row);
    for i = 1:(row)-1
        j = i+1;
        % check if this square is considered or not
        if flag(i)==1 || flag(j)==1
            continue;
        end
        
        % centers of two FIP
        ci = rp(squareCenter(i,1)).Centroid;
        cj = rp(squareCenter(j,1)).Centroid;
        for k = 1:(row)
            if i == k || j == k || flag(k)==1 
                continue;
            end
            ck = rp(squareCenter(k,1)).Centroid;
            %disp(rp(hit(k,1)))
            %disp(ck)
            
            a = pdist([ci ; cj],'euclidean');
            b = pdist([ci ; ck],'euclidean');
            c = pdist([cj ; ck],'euclidean');
            
            % Triangle inequality
            if c<a || c<b
                continue;
            end
            
            % Get the forth corner
            diff = cj - ci;
            d = ck+diff;
            
            % Get the center of the QR using its 4 corners's centriods
            centerOfQR = ((cj+ci+ck+d)/4);
            
            % check the accuracy 
            checkRatio = abs((a*a + b*b)-(c*c))/(c*c);
            
            
            if checkRatio < 0.1
                %-----------------------------------
                x = [cj(1);ci(1);ck(1);d(1)];
                y = [cj(2);ci(2);ck(2);d(2)];
                
                x = x+(x-centerOfQR(1))*0.5;
                y = y+(y-centerOfQR(2))*0.5;
                
                points = [x,y];
                img = Rimg;
                flag(i)=1;flag(j)=1;flag(k)=1;
                %-----------------------
                % get the angle to rotate
               rotationAngle = atand((points(2,2)-points(4,2))/(points(2,1)-points(4,1)));
               if points(4,1) < points(2,1)
                 rotationAngle = rotationAngle + 180;
               end
               rotationAngle = rotationAngle - 45;
               disp(rotationAngle)
               % rotate the image corresponding to the 
               [h ,w ,~] = size(img);
               QR = imrotate(img, rotationAngle);

               %------------------------
               % crop
               [rh ,rw ,~] = size(QR);
               Trans=[w/2, h/2];
               RotT=[rw/2, rh/2];
               
               R=[cosd(rotationAngle) -sind(rotationAngle); sind(rotationAngle) cosd(rotationAngle)];
               
               Trans = [Trans;Trans;Trans;Trans];
               RotTrans = [RotT;RotT;RotT;RotT];
               
               rotpoints= (points-Trans) * R+RotT ;

               rotpoints=uint32(rotpoints);
               xymax = max(rotpoints);
               xymin = min(rotpoints);
               QR = QR(xymin(2):xymax(2), xymin(1):xymax(1), :);
               figure,imshow(QR),title('Qr');
                 break;              
            end
        
        end
    end
    
end

