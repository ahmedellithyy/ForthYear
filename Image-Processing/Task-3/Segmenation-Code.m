I= imread('jump.jpg');
[h, w, c] = size(I);
Cropped = zeros(h,w,c);


figure,imshow(uint8(I));

% Crop the image to get the man only 
for i=1:336
    for j = 71:w
            Cropped(i,j,:) = I(i,j,:);
    end
end

Cropped_with_Background = Cropped;
figure,imshow(uint8(Cropped_with_Background));

for i=1:336
    for j = 71:w
        if Cropped(i,j,1) < 50 && Cropped(i,j,2) <50 && Cropped(i,j,3) <50
            Cropped(i,j,:) = 30;
        end
        if Cropped(i,j,1) < Cropped(i,j,2) &&  Cropped(i,j,2) < Cropped(i,j,3)
            Cropped(i,j,:) = 0;
        end
        
     
    end
end

figure,imshow(uint8(Cropped));

ColoredMan = uint8(Cropped);
Background = imread('jump2.jpg');
ColoredMan=im2double(ColoredMan);
Result = im2double(Background);

for i=1:h
    for j=1:w
        for z=1:3
            if(ColoredMan(i,j)>0)
                Result(i,j,z)=ColoredMan(i,j,z);
            end
        end
    end
end


figure,imshow(Result);
